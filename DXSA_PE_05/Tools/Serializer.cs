using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DXSA_PE_05.Tools
{

    public class JSON   
    {
        public String Messages = String.Empty;

        public dynamic DeserializeJSON(String json, dynamic oModel)
        {
            try
            {
                oModel = JsonConvert.DeserializeObject(json, oModel.GetType());
            }
            catch (Exception ex)
            {
                Messages = $"JSON invalido, razón: {ex.Message}";
            }

            return oModel;
        }

        public String SerializeObject(dynamic oModel)
        {
            String JSON = String.Empty;

            try
            {
                JSON = JsonConvert.SerializeObject(oModel);
            }
            catch (Exception ex)
            {
                Messages = $"JSON invalido, razón: {ex.Message}";
            }

            return JSON;
        }
    }

    public class XML    
    {
        public String Messages = String.Empty;

        public dynamic DeserializeXML(String XML, dynamic oModel)
        {
            XmlSerializer serializer = new XmlSerializer(oModel.GetType());
            TextReader reader = new StringReader(XML);

            oModel = serializer.Deserialize(reader);

            return oModel;
        }

        public dynamic DeserializeXMLMexico(String XML, dynamic oModel)
        {
            try
            {
                XML = XML.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                XML = XML.Replace("cfdi:Comprobante", "cfdi:comprobante");

                /*Namespaces regular expresion*/
                System.Text.RegularExpressions.Regex Namespaces = new System.Text.RegularExpressions.Regex(" (xmlns|xsi)[:]{0,1}(.*?)=\"(.*?)\"");

                /*Addenda regular expresion*/
                System.Text.RegularExpressions.Regex Addendas   = new System.Text.RegularExpressions.Regex("<cfdi:Addenda>(.*?)</cfdi:Addenda>");

                /*Encoding*/
                System.Text.RegularExpressions.Regex Encoding   = new System.Text.RegularExpressions.Regex("<[?]xml(.*?)>");

                System.Text.RegularExpressions.MatchEvaluator myEvaluator = new System.Text.RegularExpressions.MatchEvaluator((x) => 
                {
                    String Origin = x.Value.Replace(x.Value, String.Empty);
                    return Origin;
                });

                XML = Namespaces .Replace(XML, myEvaluator);
                XML = Addendas   .Replace(XML, myEvaluator);
                XML = Encoding   .Replace(XML, myEvaluator);

                XML = XML.Replace("cfdi:"           , String.Empty  );
                XML = XML.Replace("tfd:"            , String.Empty  );
                XML = XML.Replace("cartaporte20:"   , String.Empty  );
                XML = XML.Replace("pago20:"         , String.Empty  );
                XML = XML.Replace("implocal:"       , String.Empty  );
                XML = XML.Replace("ine:"            , String.Empty  );
                XML = XML.Replace("registrofiscal:" , String.Empty  );

                var index = XML.IndexOf('<');
                if (index > 0)
                    XML = XML.Substring(index);

                XmlSerializer serializer = new XmlSerializer(oModel.GetType());
                TextReader reader = new StringReader(XML);

                oModel = serializer.Deserialize(reader);
            }
            catch(Exception ex)
            {
                Messages = "XML invalido, razón: " + ex.Message;
            }

            return oModel;
        }

    }

}
