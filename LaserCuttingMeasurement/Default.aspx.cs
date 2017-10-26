using System;
using System.Web.UI;
using netDxf;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;

namespace LaserCuttingMeasurement
{
    public partial class _Default : Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void submitBtn_Click(object sender, EventArgs e)
        {
            if (FileUpload1.PostedFile != null && FileUpload1.PostedFile.ContentLength > 0 && (FileUpload1.FileName.EndsWith(".dxf") || FileUpload1.FileName.EndsWith(".DXF")))
            {
                //string fn = System.IO.Path.GetFileName(FileUpload1.PostedFile.FileName);
                //string SaveLocation = Server.MapPath("~/") + fn;

                System.IO.Stream fileStream = FileUpload1.PostedFile.InputStream;

                //try
                //{
                //    FileUpload1.PostedFile.SaveAs(SaveLocation);
                //}
                //catch (Exception ex)
                //{
                //    Response.Write("Error:" + ex.Message);
                //}


                //double resultPath = Measure(SaveLocation);
                double resultPath = Measure(fileStream);
                Literal1.Text = $"The file has a path length of {resultPath} inches.";
                Literal1.Visible = true;
         
            }
            else
            {
                Literal1.Text = "Please upload a .dxf file.";
                Literal1.Visible = true;
            }
        }

        private double Measure(System.IO.Stream fileInputStream)
        {
            double pathLength = 0;
            bool isBinary;
            DxfDocument dxfFile = new DxfDocument();
            String version = DxfDocument.CheckDxfFileVersion(fileInputStream, out isBinary).ToString();

            try
            {
                dxfFile = DxfDocument.Load(fileInputStream);
            }
            catch
            {
                Response.Write("Error: Unable to load .dxf file. Version = " + version);
                return 0;
            }
            if (dxfFile == null)
            {
                // current bug...
                // does not like to read in particular files I have not found yet...
                Response.Write("Error: Unable to read file properly (dxfFile == null) (v=" + version + ")");
                return 0;
            }

            double length;
            Response.Write(dxfFile.Name);
            Response.Write(dxfFile.Lines.Count);
            for (int i = 0; i < dxfFile.Lines.Count; i++)
            {
                length = LineLength(dxfFile.Lines[i].StartPoint.X, dxfFile.Lines[i].StartPoint.Y,
                                    dxfFile.Lines[i].EndPoint.X, dxfFile.Lines[i].EndPoint.Y);

                pathLength += length;
            }
            for (int i = 0; i < dxfFile.Circles.Count; i++)
            {
                length = 2 * Math.PI * dxfFile.Circles[i].Radius;

                pathLength += length;
            }
            for (int i = 0; i < dxfFile.Arcs.Count; i++)
            {
                double a1 = dxfFile.Arcs[i].StartAngle;
                double a2 = dxfFile.Arcs[i].EndAngle;
                double angle = (a1 > a2) ? (360 - a1 + a2) : (a1 - a2);
                length = Math.Abs(angle) * Math.PI / 180 * dxfFile.Arcs[i].Radius;

                pathLength += length;
            }
            for (int i = 0; i < dxfFile.LwPolylines.Count; i++)
            {
                double x1, y1, x2, y2;
                length = 0;
                for (int j = 1; j < dxfFile.LwPolylines[i].Vertexes.Count; j++)
                {
                    x1 = dxfFile.LwPolylines[i].Vertexes[j - 1].Position.X;
                    y1 = dxfFile.LwPolylines[i].Vertexes[j - 1].Position.Y;
                    x2 = dxfFile.LwPolylines[i].Vertexes[j].Position.X;
                    y2 = dxfFile.LwPolylines[i].Vertexes[j].Position.Y;
                    length = LineLength(x1, y1, x2, y2);
                }


                pathLength += length;
            }
            for (int i = 0; i < dxfFile.Ellipses.Count; i++)
            {
                var polyline = dxfFile.Ellipses[i].ToPolyline(1000);
                double x1, y1, x2, y2;
                length = 0;
                for (int j = 1; j < polyline.Vertexes.Count; j++)
                {
                    x1 = polyline.Vertexes[j - 1].Position.X;
                    y1 = polyline.Vertexes[j - 1].Position.Y;
                    x2 = polyline.Vertexes[j].Position.X;
                    y2 = polyline.Vertexes[j].Position.Y;
                    length += LineLength(x1, y1, x2, y2);
                }
                
                pathLength += length;
            }
            for (int i = 0; i < dxfFile.Splines.Count; i++)
            {
                var polyline = dxfFile.Splines[i].ToPolyline(1000);
                double x1, y1, x2, y2;
                length = 0;
                for (int j = 1; j < polyline.Vertexes.Count; j++)
                {
                    x1 = polyline.Vertexes[j - 1].Position.X;
                    y1 = polyline.Vertexes[j - 1].Position.Y;
                    x2 = polyline.Vertexes[j].Position.X;
                    y2 = polyline.Vertexes[j].Position.Y;
                    length += LineLength(x1, y1, x2, y2);
                }

                pathLength += length;
            }
            for (int i = 0; i < dxfFile.MTexts.Count; i++)
            {
                string text = dxfFile.MTexts[i].Value;
                if (text == "SOLIDWORKS Educational Product." || text == "For Instructional Use Only.")
                {
                    dxfFile.MTexts[i].Value = null;
                }
            }
            string oldFileName = System.IO.Path.GetFileNameWithoutExtension(FileUpload1.PostedFile.FileName);
            string newFileName = NewFileNameGenerator(oldFileName);
            string savePath = Server.MapPath("~/") + newFileName;
            
            try
            {
                //System.IO.Stream file = new System.IO.MemoryStream();
                //dxfFile.Save(file);
                dxfFile.Save(savePath);
                Response.Write("Edited file saved to " + savePath);

                Response.Clear();
                Response.BufferOutput = false;
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment; fileName=" + newFileName);
                Response.TransmitFile(savePath);
                Response.Flush();

            }
            catch (Exception e)
            {
                Response.Write("Error: Unable to save edited file" + e.Message);
            }
            try
            {
                SmtpClient client = new SmtpClient("smtp.service.osu.edu");
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("liew.28", "");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Port = 587;
                client.EnableSsl = true;

                MailMessage mail = new MailMessage("3DLaserCutting@osu.edu", "liew.28@osu.edu");
                mail.Subject = "FEH Laser Cutting Measurement Log";
                mail.Body = "File uploaded to site: " + savePath;
                Attachment attachFile = new Attachment(savePath, MediaTypeNames.Application.Octet);
                mail.Attachments.Add(attachFile);
                client.Send(mail);
            }
            catch (Exception e)
            {
                Response.Write("Error: Unable to send email to admin" + e.Message);
            }
            double multiplier = 1;
            string unitString = "inches";
            switch (dxfFile.DrawingVariables.InsUnits)
            {
                case netDxf.Units.DrawingUnits.Inches:
                    multiplier = 1;
                    break;
                case netDxf.Units.DrawingUnits.Feet:
                    multiplier = 1 / 12;
                    break;
                case netDxf.Units.DrawingUnits.Meters:
                    multiplier = 39.3701;
                    break;
                case netDxf.Units.DrawingUnits.Centimeters:
                    multiplier = 0.393701;
                    break;
                case netDxf.Units.DrawingUnits.Millimeters:
                    multiplier = 0.0393701;
                    break;
                default:
                    unitString = dxfFile.DrawingVariables.InsUnits.ToString();
                    break;
            }

            return pathLength * multiplier;
        }
        private static double LineLength(double x1, double y1, double x2, double y2)
        {
            double x = x1 - x2;
            double y = y1 - y2;
            return Math.Sqrt(x * x + y * y);
        }

        private string NewFileNameGenerator(string oldFileName)
        {
            string fileName = $"{teamDDL.Text}Q{quantityText.Text}M{materialDDL.SelectedValue}{oldFileName}.dxf";
            return fileName;

        }

    }
    
    
}