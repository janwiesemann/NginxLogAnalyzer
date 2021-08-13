using System;

namespace NginxLogAnalyzer
{
    internal class AccessEntry
    {
        public String Arg { get; set; }
        public String Args { get; set; }
        public String BinaryRemoteAddr { get; set; }
        public Int32 BodyBytesSent { get; set; }
        public String BytesSent { get; set; }
        public String Connection { get; set; }
        public String ConnectionRequests { get; set; }
        public String ConnectionTime { get; set; }
        public String ContentLength { get; set; }
        public String ContentType { get; set; }
        public String Cookie { get; set; }
        public String DocumentRoot { get; set; }
        public String DocumentUri { get; set; }
        public String Host { get; set; }
        public String Hostname { get; set; }
        public String Http { get; set; }
        public String HttpCookie { get; set; }
        public String HttpHost { get; set; }
        public String HttpReferer { get; set; }
        public String HttpUserAgent { get; set; }
        public String HttpVia { get; set; }
        public String HttpXForwardedFor { get; set; }
        public String Https { get; set; }
        public String IsArgs { get; set; }
        public String LimitRate { get; set; }
        public String Msec { get; set; }
        public String NginxVersion { get; set; }
        public String Pid { get; set; }
        public String Pipe { get; set; }
        public String ProxyProtocolAddr { get; set; }
        public String ProxyProtocolPort { get; set; }
        public String ProxyProtocolServerAddr { get; set; }
        public String ProxyProtocolServerPort { get; set; }
        public String QueryString { get; set; }
        public String RealpathRoot { get; set; }
        public String RemoteAddr { get; set; }
        public String RemotePort { get; set; }
        public String RemoteUser { get; set; }
        public Request Request { get; set; }
        public String RequestBody { get; set; }
        public String RequestBodyFile { get; set; }
        public String RequestCompletion { get; set; }
        public String RequestFilename { get; set; }
        public String RequestId { get; set; }
        public String RequestLength { get; set; }
        public String RequestMethod { get; set; }
        public String RequestTime { get; set; }
        public String RequestUri { get; set; }
        public String Scheme { get; set; }
        public String SentHttp { get; set; }
        public String SentHttpCacheControl { get; set; }
        public String SentHttpConnection { get; set; }
        public String SentHttpContentLength { get; set; }
        public String SentHttpContentType { get; set; }
        public String SentHttpKeepAlive { get; set; }
        public String SentHttpLastModified { get; set; }
        public String SentHttpLink { get; set; }
        public String SentHttpLocation { get; set; }
        public String SentHttpTransferEncoding { get; set; }
        public String SentTrailer { get; set; }
        public String ServerAddr { get; set; }
        public String ServerName { get; set; }
        public String ServerPort { get; set; }
        public String ServerProtocol { get; set; }
        public Int32 Status { get; set; }
        public String TcpinfoRcvSpace { get; set; }
        public String TcpinfoRtt { get; set; }
        public String TcpinfoRttvar { get; set; }
        public String TcpinfoSndCwnd { get; set; }
        public String TimeIso8601 { get; set; }
        public DateTime TimeLocal { get; set; }
        public String Uri { get; set; }
    }
}
