using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MainServer
{
    class Packet
    {
        public ushort packetNumber;
    }

    class ClientSession : PacketSession
    {
        object _lock = new object();

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            Packet packet = new Packet() { packetNumber = 1 };

            // open 여유있게 열고
            ArraySegment<byte> segment = SendBufferHelper.Open(32);
            bool success = true;
            ushort count = 0;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), packet.packetNumber);
            count += sizeof(ushort);

            success &= BitConverter.TryWriteBytes(s, count);

            // close 실제 쓴만큼 기입해서 닫음
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

            if (success)
                Send(sendBuff);

        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count);
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += sizeof(ushort);
            ushort packetNumber = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);

            //string
            ushort stringBytes = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            string name = Encoding.Unicode.GetString(s.Slice(count, stringBytes));
            count += stringBytes;

            Console.WriteLine($"[From Client] size:{size}, packetNumber:{packetNumber}, name:{name}");

            int clientKey = (int)packetNumber;

            lock (_lock)
            {
                if (Program.clients.ContainsKey(clientKey))
                {
                    Program.clients[clientKey] = this;
                    Console.WriteLine("동일한 PC번호로 중복 접속했습니다. 마지막으로 접속한 PC와 연결됩니다.");
                }
                else
                    Program.clients.Add(clientKey, this);
            }

        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
