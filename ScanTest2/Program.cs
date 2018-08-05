using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIA;

namespace ScanTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            WIA.CommonDialog dlg = new WIA.CommonDialog();
            string id = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
            //id.

            dlg.ShowAcquireImage(
                    WiaDeviceType.ScannerDeviceType,
                    WiaImageIntent.ColorIntent,
                    WiaImageBias.MaximizeQuality,
                    id,
                    true,
                    true,
                    false);
        }
    }
}
