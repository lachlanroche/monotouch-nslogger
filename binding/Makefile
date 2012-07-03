BTOUCH=/Development/MonoTouch/usr/bin/btouch
VERSION=1.0
all: NSLogger.dll


libnslogger/build/nslogger/libnslogger.a: libnslogger/nslogger/Client\ Logger/iOS/LoggerClient.h libnslogger/nslogger/Client\ Logger/iOS/LoggerClient.m libnslogger/nslogger/Client\ Logger/iOS/LoggerCommon.h
	xcodebuild -project libnslogger/nslogger.xcodeproj -target 'nslogger Universal'

NSLogger.dll: Makefile Empty.cs AssemblyInfo.cs NSLogger.cs libnslogger/build/nslogger/libnslogger.a
	$(BTOUCH) Empty.cs AssemblyInfo.cs --out=$@ --link-with=libnslogger/build/nslogger/libnslogger.a,libnslogger.a -x:NSLogger.cs

clean:
	-rm -rf libnslogger/build
	-rm -f *.dll
