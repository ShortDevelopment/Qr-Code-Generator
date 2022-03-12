using static QRCoder.PayloadGenerator;
using static QRCoder.QRCodeGenerator;

namespace QrCodeGenerator
{
    public interface IPreviewDisplay
    {
        void DiplayPayload(Payload payload, ECCLevel level = ECCLevel.Q);
        void DiplayString(string content, ECCLevel level = ECCLevel.Q);
    }
}
