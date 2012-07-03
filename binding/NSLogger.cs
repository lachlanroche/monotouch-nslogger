using System;
using System.Runtime.InteropServices;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonoTouch.NSLogger
{
	[Flags]
	public enum NSLoggerOptions
	{
		LogToConsole               = 0x01,
		BufferLogsUntilConnection  = 0x02,
		BrowseBonjour              = 0x04,
		BrowseOnlyLocalDomain      = 0x08,
		UseSSL                     = 0x10
	}

	public partial class NSLogger
	{
		private IntPtr logger;

		[DllImport("__Internal")]
		private static extern IntPtr LoggerInit();

		public NSLogger() : this(LoggerInit())
		{
		}

		private NSLogger( IntPtr logger )
		{
			this.logger = logger;
		}

		[DllImport("__Internal")]
		private static extern void LoggerSetDefaultLogger( IntPtr logger );
		[DllImport("__Internal")]
		private static extern IntPtr LoggerGetDefaultLogger();

		public static NSLogger StandardLogger
		{
			set {
				IntPtr ptr = value != null ? value.logger : IntPtr.Zero;
				LoggerSetDefaultLogger( ptr );
			}
			get {
				return new NSLogger( LoggerGetDefaultLogger());
			}
		}

		[DllImport("__Internal", EntryPoint="LoggerStart")]
		private static extern void LoggerStart( IntPtr logger );

		public void Start()
		{
			LoggerStart( logger );
		}

		[DllImport("__Internal", EntryPoint="LoggerStop")]
		private static extern void LoggerStop( IntPtr logger );

		public void Stop()
		{
			LoggerStop( logger );
		}

		[DllImport("__Internal", EntryPoint="LoggerSetViewerHost")]
		private static extern void LoggerSetViewerHost( IntPtr logger, IntPtr host, int port );

		public void SetViewerHost( string hostname, int port )
		{
			if (hostname == null) {
				throw new ArgumentNullException("hostname");
			}

			using (var ns_host = new NSString(hostname)) {
				LoggerSetViewerHost( logger, ns_host.Handle, port );
			}
		}

		[DllImport("__Internal", EntryPoint="LoggerSetupBonjour")]
		private static extern void LoggerSetupBonjour( IntPtr logger, IntPtr bonjourServiceType, IntPtr bonjourServiceName );

		public void SetupBonjour( string bonjourServiceType, string bonjourServiceName )
		{
			if (bonjourServiceName == null) {
				throw new ArgumentNullException("bonjourServiceName");
			}

			NSString ns_type = null;
			IntPtr ptr_type = IntPtr.Zero;
			if (bonjourServiceType != null) {
				ns_type = new NSString(bonjourServiceType);
				ptr_type = ns_type.Handle;
			}

			NSString ns_name = new NSString(bonjourServiceName);
			IntPtr ptr_name = ns_name.Handle;

			using (ns_type) {
				using (ns_name) {
					LoggerSetupBonjour( logger, ptr_type, ptr_name );
				}
			}
		}

		[DllImport("__Internal", EntryPoint="LoggerSetOptions")]
		private static extern void LoggerSetOptions( IntPtr logger, NSLoggerOptions options );

		public void SetViewerHost( NSLoggerOptions options )
		{
			LoggerSetOptions( logger, options );
		}

		[DllImport("__Internal", EntryPoint="LogMessageTo")]
		private static extern void LogMessage( IntPtr logger, IntPtr tag, int level, IntPtr message );

		public void Log( int level, string format, params object[] data )
		{
			Log( null, level, format, data );
		}

		public void Log( string tag, int level, string format, params object[] data )
		{
			if (format == null) {
				throw new ArgumentNullException("format");
			}

			NSString ns_tag = null;
			IntPtr ptr_tag = IntPtr.Zero;
			if (tag != null) {
				ns_tag = new NSString(tag);
				ptr_tag = ns_tag.Handle;
			}

			NSString ns_message = new NSString(string.Format(format, data));
			IntPtr ptr_message = ns_message.Handle;

			using (ns_tag) {
				using (ns_message) {
					LogMessage( logger, ptr_tag, level, ptr_message );
				}
			}
		}

		[DllImport("__Internal", EntryPoint="LogDataTo")]
		private static extern void LogData( IntPtr logger, IntPtr tag, int level, IntPtr data );

		public void Log( int level, NSData data )
		{
			Log( null, level, data );
		}

		public void Log( string tag, int level, NSData data )
		{
			if (data == null) {
				throw new ArgumentNullException("data");
			}

			NSString ns_tag = null;
			IntPtr ptr_tag = IntPtr.Zero;
			if (tag != null) {
				ns_tag = new NSString(tag);
				ptr_tag = ns_tag.Handle;
			}

			using (ns_tag) {
				LogData( logger, ptr_tag, level, data.Handle );
			}
		}

		[DllImport("__Internal", EntryPoint="LogImageDataTo")]
		private static extern void LogImageData( IntPtr logger, IntPtr tag, int level, int width, int height, IntPtr image );

		public void Log( int level, UIImage image )
		{
			Log( null, level, image );
		}

		public void Log( string tag, int level, UIImage image )
		{
			if (image == null) {
				throw new ArgumentNullException("image");
			}

			NSString ns_tag = null;
			IntPtr ptr_tag = IntPtr.Zero;
			if (tag != null) {
				ns_tag = new NSString(tag);
				ptr_tag = ns_tag.Handle;
			}

			using (ns_tag) {
				using (var png = image.AsPNG()) {
					LogImageData( logger, ptr_tag, level, (int) image.Size.Width, (int) image.Size.Height, png.Handle );
				}
			}
		}

		[DllImport("__Internal", EntryPoint="LogMarkerTo")]
		private static extern void LogMarker( IntPtr logger, IntPtr text );

		public void Marker( string text )
		{
			if (text == null) {
				throw new ArgumentNullException("text");
			}

			using (var ns_text = new NSString(text)) {
				LogMarker( logger, ns_text.Handle );
			}
		}

		[DllImport("__Internal", EntryPoint="LogStartBlockTo")]
		private static extern void LogStartBlock( IntPtr logger, IntPtr message );

		[DllImport("__Internal", EntryPoint="LogEndBlockTo")]
		private static extern void LogEndBlock( IntPtr logger );

		public IDisposable StartBlock( string format, params object[] data )
		{
			if (format == null) {
				throw new ArgumentNullException("format");
			}

			NSString ns_message = new NSString(string.Format(format, data));
			IntPtr ptr_message = ns_message.Handle;

			using (ns_message) {
				LogStartBlock( logger, ptr_message );
			}
			return new LoggerBlock( logger );
		}

		internal class LoggerBlock : IDisposable
		{
			private IntPtr logger;

			internal LoggerBlock( IntPtr logger )
			{
				this.logger = logger;
			}

			void IDisposable.Dispose()
			{
				LogEndBlock( logger );
			}
		}
	}
}
