using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels.Ipc;

namespace RealtimeViewer.Ipc
{
    public enum RtvIpcRequest
    {
        None = 0,
        ActivateWindow = 123,   // メインウィンドウをアクティブにするんだ！！！
    }

    /// <summary>
    /// プロセス間通信用のペイロード。
    /// </summary>
    public class RtvIpcMessage : MarshalByRefObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override object InitializeLifetimeService()
        {
            // タイムアウトしない。
            return null;
        }

        public RtvIpcMessage()
        {
            _ipcRequest = RtvIpcRequest.None;
        }

        private RtvIpcRequest _ipcRequest;
        public RtvIpcRequest Request
        {
            get
            {
                return _ipcRequest;
            }

            set
            {
                if (_ipcRequest != value)
                {
                    _ipcRequest = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// リクエストをクリア。
        /// </summary>
        public void Clear()
        {
            _ipcRequest = RtvIpcRequest.None;
        }
    }

    /// <summary>
    /// IPCサーバー。口を開けて待つ係。
    /// </summary>
    /// <remarks>
    /// .NET 5 では System.Runtime.Remoting.Channels.Ipc が非対応で、動かなくなる。
    /// テキトーにNamed Pipeで作り直すとかやってくれたまえ。
    /// </remarks>
    /// <see cref="https://araramistudio.jimdo.com/2020/05/11/c-でプロセス間通信-ipc通信/"/>
    public class RtvIpcMessageServer : IDisposable
    {
        public const string SERVER_NAME = @"IsdtRealtimeViewerIpc";
        public const string API_NAME_REQUEST = @"request";

        private IpcServerChannel channel;
        private RtvIpcMessage request;

        public RtvIpcMessage Request
        {
            get
            {
                return request;
            }

            private set
            {

            }
        }

        public RtvIpcMessageServer()
        {
            channel = new IpcServerChannel(SERVER_NAME);
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, true);

            request = new RtvIpcMessage();
            System.Runtime.Remoting.RemotingServices.Marshal(request, API_NAME_REQUEST, typeof(RtvIpcMessage));
        }

        public void Dispose()
        {
            System.Runtime.Remoting.RemotingServices.Disconnect(request);
            System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(channel);
        }
    }

    /// <summary>
    /// IPCクライアント。要求を送り付ける係(の予定)。
    /// </summary>
    public class RtvIPcMessageClient : IDisposable
    {
        private IpcClientChannel channel;
        private RtvIpcMessage request;

        public RtvIPcMessageClient()
        {
            channel = new IpcClientChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, true);

            var url = $"ipc://{RtvIpcMessageServer.SERVER_NAME}/{RtvIpcMessageServer.API_NAME_REQUEST}";
            request = (RtvIpcMessage)Activator.GetObject(typeof(RtvIpcMessage), url);
        }

        public void Dispose()
        {
            System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(channel);
        }

        public bool Request(RtvIpcRequest requestType)
        {
            bool ret = false;
            if (request != null)
            {
                request.Request = requestType;
                ret = true;
            }

            return ret;
        }
    }
}
