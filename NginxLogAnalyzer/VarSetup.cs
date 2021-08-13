using System;
using System.Collections.Generic;
using NginxLogAnalyzer.Parser;

namespace NginxLogAnalyzer
{
    internal static partial class Setup
    {
        internal static List<IVariable> GetFormatVariables()
        {
            List<IVariable> ret = GetInstancesOfType<IVariable>();

            ret.Add(new StringVariable("arg_", (v, e) => e.Arg = v));
            ret.Add(new StringVariable("args", (v, e) => e.Args = v));
            ret.Add(new StringVariable("binary_remote_addr", (v, e) => e.BinaryRemoteAddr = v));
            ret.Add(new IntVariable("body_bytes_sent", (v, e) => e.BodyBytesSent = v));
            ret.Add(new StringVariable("bytes_sent", (v, e) => e.BytesSent = v));
            ret.Add(new StringVariable("connection", (v, e) => e.Connection = v));
            ret.Add(new StringVariable("connection_requests", (v, e) => e.ConnectionRequests = v));
            ret.Add(new StringVariable("connection_time", (v, e) => e.ConnectionTime = v));
            ret.Add(new StringVariable("content_length", (v, e) => e.ContentLength = v));
            ret.Add(new StringVariable("content_type", (v, e) => e.ContentType = v));
            ret.Add(new StringVariable("cookie_", (v, e) => e.Cookie = v));
            ret.Add(new StringVariable("document_root", (v, e) => e.DocumentRoot = v));
            ret.Add(new StringVariable("document_uri", (v, e) => e.DocumentUri = v));
            ret.Add(new StringVariable("host", (v, e) => e.Host = v));
            ret.Add(new StringVariable("hostname", (v, e) => e.Hostname = v));
            ret.Add(new StringVariable("http_", (v, e) => e.Http = v));
            ret.Add(new StringVariable("http_cookie", (v, e) => e.HttpCookie = v));
            ret.Add(new StringVariable("http_host", (v, e) => e.HttpHost = v));
            ret.Add(new StringVariable("http_referer", (v, e) => e.HttpReferer = v));
            ret.Add(new StringVariable("http_user_agent", (v, e) => e.HttpUserAgent = v));
            ret.Add(new StringVariable("http_via", (v, e) => e.HttpVia = v));
            ret.Add(new StringVariable("http_x_forwarded_for", (v, e) => e.HttpXForwardedFor = v));
            ret.Add(new StringVariable("https", (v, e) => e.Https = v));
            ret.Add(new StringVariable("is_args", (v, e) => e.IsArgs = v));
            ret.Add(new StringVariable("limit_rate", (v, e) => e.LimitRate = v));
            ret.Add(new StringVariable("msec", (v, e) => e.Msec = v));
            ret.Add(new StringVariable("nginx_version", (v, e) => e.NginxVersion = v));
            ret.Add(new StringVariable("pid", (v, e) => e.Pid = v));
            ret.Add(new StringVariable("pipe", (v, e) => e.Pipe = v));
            ret.Add(new StringVariable("proxy_protocol_addr", (v, e) => e.ProxyProtocolAddr = v));
            ret.Add(new StringVariable("proxy_protocol_port", (v, e) => e.ProxyProtocolPort = v));
            ret.Add(new StringVariable("proxy_protocol_server_addr", (v, e) => e.ProxyProtocolServerAddr = v));
            ret.Add(new StringVariable("proxy_protocol_server_port", (v, e) => e.ProxyProtocolServerPort = v));
            ret.Add(new StringVariable("query_string", (v, e) => e.QueryString = v));
            ret.Add(new StringVariable("realpath_root", (v, e) => e.RealpathRoot = v));
            ret.Add(new StringVariable("remote_addr", (v, e) => e.RemoteAddr = v));
            ret.Add(new StringVariable("remote_port", (v, e) => e.RemotePort = v));
            ret.Add(new StringVariable("remote_user", (v, e) => e.RemoteUser = v));
            ret.Add(new StringVariable("request", (v, e) => e.Request = Request.ParseRequest(v)));
            ret.Add(new StringVariable("request_body", (v, e) => e.RequestBody = v));
            ret.Add(new StringVariable("request_body_file", (v, e) => e.RequestBodyFile = v));
            ret.Add(new StringVariable("request_completion", (v, e) => e.RequestCompletion = v));
            ret.Add(new StringVariable("request_filename", (v, e) => e.RequestFilename = v));
            ret.Add(new StringVariable("request_id", (v, e) => e.RequestId = v));
            ret.Add(new StringVariable("request_length", (v, e) => e.RequestLength = v));
            ret.Add(new StringVariable("request_method", (v, e) => e.RequestMethod = v));
            ret.Add(new StringVariable("request_time", (v, e) => e.RequestTime = v));
            ret.Add(new StringVariable("request_uri", (v, e) => e.RequestUri = v));
            ret.Add(new StringVariable("scheme", (v, e) => e.Scheme = v));
            ret.Add(new StringVariable("sent_http_", (v, e) => e.SentHttp = v));
            ret.Add(new StringVariable("sent_http_cache_control", (v, e) => e.SentHttpCacheControl = v));
            ret.Add(new StringVariable("sent_http_connection", (v, e) => e.SentHttpConnection = v));
            ret.Add(new StringVariable("sent_http_content_length", (v, e) => e.SentHttpContentLength = v));
            ret.Add(new StringVariable("sent_http_content_type", (v, e) => e.SentHttpContentType = v));
            ret.Add(new StringVariable("sent_http_keep_alive", (v, e) => e.SentHttpKeepAlive = v));
            ret.Add(new StringVariable("sent_http_last_modified", (v, e) => e.SentHttpLastModified = v));
            ret.Add(new StringVariable("sent_http_link", (v, e) => e.SentHttpLink = v));
            ret.Add(new StringVariable("sent_http_location", (v, e) => e.SentHttpLocation = v));
            ret.Add(new StringVariable("sent_http_transfer_encoding", (v, e) => e.SentHttpTransferEncoding = v));
            ret.Add(new StringVariable("sent_trailer_", (v, e) => e.SentTrailer = v));
            ret.Add(new StringVariable("server_addr", (v, e) => e.ServerAddr = v));
            ret.Add(new StringVariable("server_name", (v, e) => e.ServerName = v));
            ret.Add(new StringVariable("server_port", (v, e) => e.ServerPort = v));
            ret.Add(new StringVariable("server_protocol", (v, e) => e.ServerProtocol = v));
            ret.Add(new IntVariable("status", (v, e) => e.Status = v));
            ret.Add(new StringVariable("tcpinfo_rcv_space", (v, e) => e.TcpinfoRcvSpace = v));
            ret.Add(new StringVariable("tcpinfo_rtt", (v, e) => e.TcpinfoRtt = v));
            ret.Add(new StringVariable("tcpinfo_rttvar", (v, e) => e.TcpinfoRttvar = v));
            ret.Add(new StringVariable("tcpinfo_snd_cwnd", (v, e) => e.TcpinfoSndCwnd = v));
            ret.Add(new StringVariable("time_iso8601", (v, e) => e.TimeIso8601 = v));
            ret.Add(new DateTimeVariable("time_local", (v, e) => e.TimeLocal = v));
            ret.Add(new StringVariable("uri", (v, e) => e.Uri = v));

            return ret;
        }
    }
}
