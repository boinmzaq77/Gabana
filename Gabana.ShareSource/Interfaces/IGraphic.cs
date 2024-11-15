using Gabana.ShareSource.ClassStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource.Interfaces
{
    public interface IGraphic
    {
        byte[] DrawString();
        byte[] DrawImageFromXml(string xmlName, ParamSlip paramSlip);
    }
}
