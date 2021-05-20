using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace huaanClient
{
    class PrinPDF
    {
        /// <summary>
        /// 通过id获取相关需要的信息并生成pdf  
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns>2代表成功，1代表未注册line接口，0代表发生错误是否已经打开相关的pdf</returns>
        public static void  prinpdf(string id)
        {
            string tishi = "ヒント"; 
            if (!ApplicationData.LanguageSign.Contains("日本語"))
            {
                MessageBox.Show("Error", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            //检测目录的
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("生成に失敗しました", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            string info = getstaffinfo(id);
            JArray srjo = (JArray)JsonConvert.DeserializeObject(info);
            if (srjo.Count == 0)
            {
                MessageBox.Show("生成に失敗しました", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            string name = srjo[0]["name"].ToString();
            string line_code = srjo[0]["line"].ToString();
            string zumin = srjo[0]["zumin"].ToString();

            //先获取二维码照片
            string imgeurlinfo = GetData.getlineQRcode();
            JArray imgeurlinfojo = (JArray)JsonConvert.DeserializeObject(imgeurlinfo);
            if (imgeurlinfojo.Count == 0)
            {
                MessageBox.Show("生成に失敗しました", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            string imgeurl = imgeurlinfojo[0]["lineRQcode"].ToString();
            if (string.IsNullOrEmpty(imgeurl))
            {
                MessageBox.Show("line関連の二次元コードをお申し込みにならない場合は、先に二次元コードをお申し込みください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            else
            {
                if (!System.IO.File.Exists(imgeurl))
                {
                    MessageBox.Show("line二次元コードは存在しません。二次元コードを新規にアップロードしてください。", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                    return;
                }   
            }
            if (line_code.Length < 6)
            {
                MessageBox.Show("LINE認証を行ってから操作してください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(line_code)  || string.IsNullOrEmpty(zumin) )
            {
                MessageBox.Show("LINEインターフェースに関する情報が見つかりません。登録されているか確認してください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            int re=CreatePdfSetInfo(name, line_code, zumin, imgeurl);
            if (re==0)
            {
                MessageBox.Show("出力完了", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            else if(re==1)
            {
                return;
            }
            else if (re==2)
            {
                MessageBox.Show("生成に失敗した場合は、ファイルが存在していて開いていることを確認してください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
        }


        public static void prinpdfforlineEmail(string id)
        {
            string tishi = "ヒント";
            if (!ApplicationData.LanguageSign.Contains("日本語"))
            {
                MessageBox.Show("Error", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            //检测目录的
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("生成に失敗しました", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            string info = getstaffinfo(id);
            JArray srjo = (JArray)JsonConvert.DeserializeObject(info);
            if (srjo.Count == 0)
            {
                MessageBox.Show("生成に失敗しました", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            string name = srjo[0]["name"].ToString();
            string linecodemail = srjo[0]["linecodemail"].ToString();
            string zumin = srjo[0]["zumin"].ToString();

            //先获取二维码照片
            string imgeurlinfo = GetData.getlineEmailQRcode();
            JArray imgeurlinfojo = (JArray)JsonConvert.DeserializeObject(imgeurlinfo);
            if (imgeurlinfojo.Count == 0)
            {
                MessageBox.Show("生成に失敗しました", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            string imgeurl = imgeurlinfojo[0]["lineRQcodeEmail"].ToString();
            if (string.IsNullOrEmpty(imgeurl))
            {
                MessageBox.Show("line関連の二次元コードをお申し込みにならない場合は、先に二次元コードをお申し込みください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            else
            {
                if (!System.IO.File.Exists(imgeurl))
                {
                    MessageBox.Show("line二次元コードは存在しません。二次元コードを新規にアップロードしてください。", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                    return;
                }
            }
            if (linecodemail.Length < 6)
            {
                MessageBox.Show("Mail認証を行ってから操作してください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(linecodemail) || string.IsNullOrEmpty(zumin))
            {
                MessageBox.Show("LINEインターフェースに関する情報が見つかりません。登録されているか確認してください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            int re = CreatePdfSetInfoEmail(name, linecodemail, zumin, imgeurl);
            if (re == 0)
            {
                MessageBox.Show("出力完了", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            else if (re == 1)
            {
                return;
            }
            else if (re == 2)
            {
                MessageBox.Show("生成に失敗した場合は、ファイルが存在していて開いていることを確認してください", tishi, MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
        }


        public static string getstaffinfo(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            string re = GetData.getstaffinfo(id);
            return re;
        }

        private static Font font1;

        /// <summary>
        /// 设置页面大小、作者、标题等相关信息设置
        /// </summary>
        private static int CreatePdfSetInfo( string name,string line_code, string Group_name, string imgeurl)
        {
            try
            {
                string fileName = string.Empty;
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = name +"-"+ line_code;
                dlg.DefaultExt = ".pdf";
                dlg.Filter = "Text documents (.pdf)|*.pdf";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    fileName = dlg.FileName;


                    BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\MSYH.TTC,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    font1 = new Font(bf, 12);
                    //设置页面大小
                    iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(620f, 800f);

                    //设置边界
                    Document document = new Document(pageSize, 70f, 70f, 50f, 50f);
                    PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                    // 添加文档信息
                    document.AddTitle("PDFInfo");
                    //document.AddSubject("Demo of PDFInfo");
                    //document.AddKeywords("Info, PDF, Demo");
                    //document.AddCreator("SetPdfInfoDemo");
                    document.Open();
                    // 添加文档内容
                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 15, 1);
                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.pdftitle, 1, 40, 50, 1.5f));
                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 12);

                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 12);
                    document.Add(HeaderAndFooterEvent.AddParagraph(name + ApplicationData.rows1, 0, 10, 0, 1.5f));

                    //添加一个空段落来占位，五个参数分别为：内容，对齐方式（1为居中，0为居左，2为居右），段后空行数，段前空行数，行间距
                    document.Add(HeaderAndFooterEvent.AddParagraph(Group_name, 2, 40, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows2 + Group_name + ApplicationData.rows3, 0, 0, 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows4
                        , 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows5
                        , 0, 10, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows6, 0, 10, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows7, 0, 0, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows8
                        , 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows9
                        , 0, 25, 0, 1.5f));

                    document.Add(AddImage(imgeurl, 1, document.RightMargin, document.Bottom));
                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows10
                        , 1, 0, 10, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph(
                        ApplicationData.rows11, 1, 20, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph(ApplicationData.rows12, 1, 15, 0, 1.5f));
                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 40, 1);
                    document.Add(HeaderAndFooterEvent.AddParagraph(line_code, 1, 1.5f));
                    document.Close();
                    return 0;
                }
                return 1;
            }
            catch
            {
                return 2;
            }
            
        }

        private static int CreatePdfSetInfoEmail(string name, string linecodemail, string Group_name, string imgeurl)
        {
            try
            {
                string fileName = string.Empty;
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = name + "-" + linecodemail;
                dlg.DefaultExt = ".pdf";
                dlg.Filter = "Text documents (.pdf)|*.pdf";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    fileName = dlg.FileName;


                    BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\MSYH.TTC,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    font1 = new Font(bf, 12);
                    //设置页面大小
                    iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(620f, 800f);

                    //设置边界
                    Document document = new Document(pageSize, 70f, 70f, 50f, 50f);
                    PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                    // 添加文档信息
                    document.AddTitle("PDFInfo");
                    //document.AddSubject("Demo of PDFInfo");
                    //document.AddKeywords("Info, PDF, Demo");
                    //document.AddCreator("SetPdfInfoDemo");
                    //document.AddAuthor("焦涛");
                    document.Open();
                    // 添加文档内容
                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 15, 1);
                    document.Add(HeaderAndFooterEvent.AddParagraph("登下校見守りサービスの案内", 1, 40, 50, 1.5f));
                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 12);

                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 12);
                    document.Add(HeaderAndFooterEvent.AddParagraph(name + "の保護者様へ", 0, 10, 0, 1.5f));

                    //添加一个空段落来占位，五个参数分别为：内容，对齐方式（1为居中，0为居左，2为居右），段后空行数，段前空行数，行间距
                    document.Add(HeaderAndFooterEvent.AddParagraph(Group_name, 2, 40, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph("この度、" + Group_name + " では新型コロナウィルス感染症対策の一策として、", 0, 0, 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph("登下校時に子どもたちの体温チェックを行える HEAT CHECK を導入いたしました。"
                        , 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph("HEAT CHECK には子どもたちの登下校通知を行えるサービスもあり、"
                        , 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph("こちらを登録していただくとお子様が HEAT CHECK で体温チェックを行った際に、"
                        , 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph("メールで登下校のお知らせをお届けできます。"
                        , 0, 10, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph("①スマートフォンで下記の QR コードを読み込んでください。" +
                        "（読み込むと登下校見守りサービスのページが表示されます。）", 0, 10, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph("②登録画面にお名前、通知を受け取りたいメールアドレスを入力し、" +
                        "登録ボタンを押してください。", 0, 10, 0, 1.5f));

                    document.Add(HeaderAndFooterEvent.AddParagraph("③しばらくすると登録確認メールが届きますので、下記の６桁の認証コードを入力し" +
                        "間違いなければ確認ボタンを押してください。", 0, 0, 0, 1.5f));

                    //document.Add(HeaderAndFooterEvent.AddParagraph("下記の認証コードを入力してください。"
                    //    , 0, 1.5f));
                    document.Add(HeaderAndFooterEvent.AddParagraph("※メールアドレスまたは、６桁の認証コードが間違っていると正確に登録できませんので、失敗した場合は再度手順①から操作されてください。"
                        , 0, 6, 0, 1.5f));

                    document.Add(AddImage(imgeurl, 1, document.RightMargin, document.Bottom));
                    document.Add(HeaderAndFooterEvent.AddParagraph("こちらの QR コードを読み取って下さい"
                        , 1, 6, 6, 1.5f));

                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 15, 0);
                    document.Add(HeaderAndFooterEvent.AddParagraph("認証コード ※認証コードは絶対に他人に知られないようにして下さい", 1, 6, 6, 1.5f));
                    HeaderAndFooterEvent.SetFont(BaseColor.DARK_GRAY, "宋体", 40, 1);
                    document.Add(HeaderAndFooterEvent.AddParagraph(linecodemail, 1, 1.5f));
                    document.Close();
                    return 0;
                }
                return 1;
            }
            catch
            {
                return 2;
            }

        }
        public static Paragraph AddParagraph(string content, int Alignment, float SpacingAfter, float SpacingBefore, float MultipliedLeading)
        {
            Paragraph pra = new Paragraph(content, font1);
            pra.Alignment = Alignment;
            pra.SpacingAfter = SpacingAfter;
            pra.SpacingBefore = SpacingBefore;
            pra.MultipliedLeading = MultipliedLeading;
            return pra;
        }

        public static Paragraph AddParagraph(string content, int Alignment, float MultipliedLeading)
        {
            Paragraph pra = new Paragraph(content, font1);
            pra.Alignment = Alignment;
            pra.MultipliedLeading = MultipliedLeading;
            return pra;
        }

        public static void AddPhrase(PdfWriter writer, Document document, string content, float marginLift, float marginBottom)
        {
            Phrase phrase = new Phrase(content, font1);

            PdfContentByte cb = writer.DirectContent;
            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, phrase,
                       marginLift + document.LeftMargin, marginBottom, 0);

        }

        #region 添加图片
        /// <summary>
        /// 添加图片
        /// <param name="Alignment">对齐方式 (0/1/2)</param>
        /// <param name="marginRight">右边距</param>
        /// <param name="marginBottom">下边距</param>
        /// </summary>
        public static iTextSharp.text.Image AddImage(string path, int Alignment, float marginRight, float marginBottom)
        {
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(new Uri(path));
            img.ScaleAbsolute(100, 100);

            img.Alignment = Alignment;
            //等比缩放，宽与高的缩放系数哪个大就取哪一个（比如高的系数是0.8，宽的是0.7，则取0.7。这样图片就不会超出页面范围）
            if (img.Width > img.Height)
            {
                //这里计算图片的缩放系数，因为图片width>height,所以将图片旋转90度以适应页面，计算缩放系数的时候宽与高对调
                float PageHeight = PageSize.A4.Height - marginBottom * 3;
                double percentHeight = Math.Round(PageHeight / img.Width, 2);

                float PageWidth = PageSize.A4.Width - marginRight * 2;
                double percentWidth = Math.Round(PageWidth / img.Height, 2);

                double percent = percentHeight > percentWidth ? percentWidth : percentHeight;
                img.ScalePercent((float)percent * 30);
            }
            else
            {
                float PageHeight = PageSize.A4.Height - marginBottom * 3;
                double percentHeight = Math.Round(PageHeight / img.Height, 2);

                float PageWidth = PageSize.A4.Width - marginRight * 2;
                double percentWidth = Math.Round(PageWidth / img.Width, 2);

                double percent = percentHeight > percentWidth ? percentWidth : percentHeight;
                img.ScalePercent((float)percent * 30);
            }
            return img;
        }
        #endregion
    }
}
