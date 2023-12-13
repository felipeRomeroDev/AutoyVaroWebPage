using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace AutoyVaro.Helpers.UtilitiesHelper
{


    public class SubirArchivo
    {
        private static char DirSeparador = System.IO.Path.DirectorySeparatorChar;
        private static string SUFIJO = "_";
        private static string FilePath = SettingsHelper.getRutaDirectorio();

        public static string UploadFile(HttpPostedFileBase file, int idCredito, int idCliente, string NombreDeArchivo)
        {
            if (file == null) return "";
            if (!(file.ContentLength > 0)) return "";

            string fileName = file.FileName;
            string fileExt = Path.GetExtension(file.FileName);

            string filePathCredito = FilePath + DirSeparador + idCliente.ToString() + DirSeparador + idCredito.ToString();

            if (fileExt == null) return "";

            if (!Directory.Exists(filePathCredito))
            {
                Directory.CreateDirectory(filePathCredito);
            }

            string path = filePathCredito + DirSeparador + NombreDeArchivo + fileExt;

            file.SaveAs(Path.GetFullPath(path));
            return NombreDeArchivo;

        }


        public static string UploadFile(HttpPostedFileBase file, string NombreDeArchivo)
        {
            
            String RuraSitio = HttpContext.Current.Server.MapPath("~/IMG");
            


            if (file == null) return "";
            if (!(file.ContentLength > 0)) return "";

            string fileName = file.FileName;
            string fileExt = Path.GetExtension(file.FileName);

            string filePathCredito = RuraSitio;

            if (fileExt == null) return "";

            if (!Directory.Exists(filePathCredito))
            {
                Directory.CreateDirectory(filePathCredito);
            }

            string path = filePathCredito + DirSeparador + NombreDeArchivo + fileName;

            file.SaveAs(Path.GetFullPath(path));
            return NombreDeArchivo + fileName;

        }


        public static string UploadFileFTP(HttpPostedFileBase file, int idCredito, int idCliente, string NombreDeArchivo)
        {
            try
            {

                string fileExt = Path.GetExtension(file.FileName);
                if (fileExt == null) return "";

                NombreDeArchivo = idCliente + SUFIJO + idCredito + SUFIJO + NombreDeArchivo + fileExt;

                //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://procasa.gotdns.com/DocumentosCreseAuto" + DirSeparador + NombreDeArchivo));
                //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://201.108.122.26/DocumentosCreseAuto" + DirSeparador + NombreDeArchivo));
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://win5190.site4now.net" + "/" + NombreDeArchivo));
                


                request.UsePassive = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                //request.Credentials = new NetworkCredential("adminftpprocasa", "jugodelvalle");
                request.Credentials = new NetworkCredential("autoyvaro", "Lania2012");
                request.EnableSsl = false;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.Proxy = null;

                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] buffer = b.ReadBytes(file.ContentLength);

                request.ContentLength = file.ContentLength;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, file.ContentLength);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                //NombreDeArchivo = response.StatusDescription;
                return NombreDeArchivo;
            }
            catch (WebException e)
            {
                FtpWebResponse response = (FtpWebResponse)e.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    //throw new Exception("Error al subir a ftp://procasa.gotdns.com" + e.Message, e);
                    //throw new Exception("Error al subir a ftp://201.108.122.26" + e.Message, e);
                    throw new Exception("Error al subir a win5190.site4now.net" + e.Message, e);

                    
                }
                else
                {
                    response.Close();
                    //throw new Exception("Error al subir a  ftp://procasa.gotdns.com " + e.Message, e);
                    //throw new Exception("Error al subir a  ftp://201.108.122.26" + e.Message, e);
                    throw new Exception("Error al subir a  win5190.site4now.net" + e.Message, e);

                    
                }

            }
            catch (Exception e)
            {
                //throw new Exception("Error al subir a  ftp://201.108.122.26 " + e.Message, e);
                //throw new Exception("Error al subir a  ftp://procasa.gotdns.com " + e.Message, e);
                throw new Exception("Error al subir a  ftp://procasadexalapa.com.mx" + e.Message, e);
                

            }
        }


        public static string UploadFileFTPPolitica(HttpPostedFileBase file, string NombreDeArchivo)
        {
            try
            {

                string fileExt = Path.GetExtension(file.FileName);
                if (fileExt == null) return "";

                NombreDeArchivo = NombreDeArchivo + fileExt;

                //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://procasa.gotdns.com/DocumentosCreseAuto" + DirSeparador + NombreDeArchivo));
                //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://201.108.122.26/DocumentosCreseAuto" + DirSeparador + NombreDeArchivo));
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://procasadexalapa.com.mx/DocumentosCreseAuto" + DirSeparador + NombreDeArchivo));



                request.UsePassive = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                //request.Credentials = new NetworkCredential("adminftpprocasa", "jugodelvalle");
                request.Credentials = new NetworkCredential("autoyvaro@procasadexalapa.com.mx", "Lania2012");
                request.EnableSsl = false;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.Proxy = null;

                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] buffer = b.ReadBytes(file.ContentLength);

                request.ContentLength = file.ContentLength;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, file.ContentLength);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                //NombreDeArchivo = response.StatusDescription;
                return NombreDeArchivo;
            }
            catch (WebException e)
            {
                FtpWebResponse response = (FtpWebResponse)e.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    //throw new Exception("Error al subir a ftp://procasa.gotdns.com" + e.Message, e);
                    //throw new Exception("Error al subir a ftp://201.108.122.26" + e.Message, e);
                    throw new Exception("Error al subir a ftp://procasadexalapa.com.mx" + e.Message, e);


                }
                else
                {
                    response.Close();
                    //throw new Exception("Error al subir a  ftp://procasa.gotdns.com " + e.Message, e);
                    //throw new Exception("Error al subir a  ftp://201.108.122.26" + e.Message, e);
                    throw new Exception("Error al subir a  ftp://procasadexalapa.com.mx" + e.Message, e);


                }

            }
            catch (Exception e)
            {
                //throw new Exception("Error al subir a  ftp://201.108.122.26 " + e.Message, e);
                //throw new Exception("Error al subir a  ftp://procasa.gotdns.com " + e.Message, e);
                throw new Exception("Error al subir a  ftp://procasadexalapa.com.mx" + e.Message, e);


            }
        }


        public static MemoryStream DownloadFileFTP(String path = "")
        {
            try
            {
                if (String.IsNullOrEmpty(path)) return null;
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(path);
                request.UsePassive = true;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("autoyvaro", "Lania2012");
                request.UseBinary = true;
                request.Proxy = null;
                request.KeepAlive = false;
                request.EnableSsl = false;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    MemoryStream ms = new MemoryStream();
                    using (Stream rs = (Stream)response.GetResponseStream())
                    {
                        rs.CopyTo(ms);
                    }
                    return ms;
                }
            }
            catch (Exception e)
            {
                //throw new Exception("Error al descargar a  ftp://procasa.gotdns.com " + e.Message, e);
                //throw new Exception("Error al descargar a  ftp://201.108.122.26 " + e.Message, e);
                throw new Exception("Error al descargar a  ftp://procasadexalapa.com.mx " + e.Message, e);

                
            }
        }

        public static void DeleteFile(String NombreDeArchivo)
        {

            try
            {
                //Uri serverUri = new Uri("ftp://procasa.gotdns.com/DocumentosCreseAuto/" + NombreDeArchivo);
                //Uri serverUri = new Uri("ftp://201.108.122.26/DocumentosCreseAuto/" + NombreDeArchivo);
                Uri serverUri = new Uri("ftp://procasadexalapa.com.mx/DocumentosCreseAuto/" + NombreDeArchivo);
                

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);

                //Credenciales
                request.Credentials = new NetworkCredential("autoyvaro@procasadexalapa.com.mx", "Lania2012");
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException e)
            {
                Console.Write(e.Message);
            }
        }
    }
}
