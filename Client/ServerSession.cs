using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Packet
    {
        public ushort packetNumber;
        public string name;
    }

    class ServerSession : PacketSession
    {
        ushort number = ushort.Parse(Settings.settings["number"]);

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            Packet packet = new Packet() { packetNumber = number, name = "테스트이름"};

            // open 여유있게 열고
            ArraySegment<byte> segment = SendBufferHelper.Open(32);
            bool success = true;
            ushort count = 0;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), packet.packetNumber);
            count += sizeof(ushort);

            //string GetBytes로 문자열을 +sizeof(ushort)로 문자열size넣을 공간을 띄워놓고 segment에 넣은 뒤에 이후 사이즈 삽입 
            ushort stringBytes = (ushort) Encoding.Unicode.GetBytes(packet.name, 0, packet.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), stringBytes);
            count += sizeof(ushort);
            count += stringBytes;

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
            ushort recvCount = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            recvCount += sizeof(ushort);
            ushort packetNumber = BitConverter.ToUInt16(s.Slice(recvCount, s.Length - recvCount));
            recvCount += sizeof(ushort);

            Console.WriteLine($"[From Server] size:{size}, packetNumber:{packetNumber}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
