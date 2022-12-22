using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Font = iTextSharp.text.Font;
using UnityEngine.UI;
using Image = iTextSharp.text.Image;

public class PDFReport : MonoBehaviour, IDisposable
{
    public Button bt;

    private Document document;
    private PdfWriter pdfWriter;

    private string filePath;
  
                           
    private string fontPath;

    private string imagePath;

    private AndroidJavaObject call;


    private void Start()
    {
        filePath = Application.persistentDataPath + "/test.pdf";
       
        imagePath = Application.persistentDataPath + "/图片.png";

        fontPath = Application.persistentDataPath + "/tt.otf";
        
        StartCoroutine(CopyFile(Application.streamingAssetsPath + "/tt.otf", Application.persistentDataPath + "/tt.otf"));

        StartCoroutine(CopyFile(Application.streamingAssetsPath + "/图片.png", Application.persistentDataPath + "/图片.png"));


        bt.onClick.AddListener(()=> {
            Initialized(filePath);

            Font font = GetFont(fontPath, 20, Font.BOLD, 0, 0, 0);
            AddTitle("导出PDF", 1, font);
            Image image = GetImage(imagePath, 500,200);
            image.Alignment = Element.ALIGN_JUSTIFIED;
            AddImage(image);

          

            Paragraph paragraph = GetParagraph("块（Chunks）是容纳文本的最小容器，就像ASP.Net中的<asp:Label>一样，可以使或者Environment.NewLine,或者Chunk.NEWLINE作为给Chunk对象赋值的一部分。Chunk有一系列方法允许你为文本设置样式，比如setUnderLine(), setBackGround(), 和 setTextRise()以及一些构造函数来设置字体类型以及风格。", font);

            paragraph.FirstLineIndent = 20;
            paragraph.SetLeading(3, 2);


            document.Add(paragraph);

            List list = new List(List.UNORDERED, 10f);
            list.SetListSymbol("\u2022");
            list.IndentationLeft = 30f;

            list.Add("One");
            list.Add("Two");
            list.Add("Three");
            list.Add("Four");
            list.Add("Five");

            Paragraph para = new Paragraph();
            para.Add("Lists");
            document.Add(para);
            document.Add(list);

            List<string> strList = new List<string>() {

        "你好",
        "Hello"
        };

            List list1 = GetList(strList, font);


            document.Add(list1);


            PdfPTable getPdfPTable = GetPdfPTable(6);

            PdfPCell pdfPCell = GetPdfPCell("课程表", font);
            pdfPCell.Colspan = 6;//跨6列
            pdfPCell.HorizontalAlignment = 1;

            getPdfPTable.AddCell(pdfPCell);
            getPdfPTable.AddCell(GetPhrase("语文", font));
            getPdfPTable.AddCell(GetPhrase("100分", font));
            getPdfPTable.AddCell(GetPhrase("数学", font));
            getPdfPTable.AddCell(GetPhrase("90分", font));
            getPdfPTable.AddCell(GetPhrase("英语", font));
            getPdfPTable.AddCell(GetPhrase("120分", font));

            getPdfPTable.AddCell(GetPhrase("生物", font));
            getPdfPTable.AddCell(GetPhrase("130分", font));
            getPdfPTable.AddCell(GetPhrase("地理", font));
            getPdfPTable.AddCell(GetPhrase("90分", font));
            getPdfPTable.AddCell(GetPhrase("历史", font));
            getPdfPTable.AddCell(GetPhrase("140分", font));


            document.NewPage();
            document.Add(getPdfPTable);





            document.Close();
            pdfWriter.Close();

          

#if PLATFORM_ANDROID&&!UNITY_EDITOR


            if (call == null)
            {
                AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.example.mylibrary.OpenAndroidFile");

                call = androidJavaClass.CallStatic<AndroidJavaObject>("GetInstance");

            }

            call.CallStatic("openAndroidFile", filePath);

#else
            Application.OpenURL(filePath);

#endif
        });


    
        


    }


    /// <summary>
    /// 初始化PDF文档
    /// </summary>
    /// <param name="filePath"></param>
    public void Initialized(string filePath)
    {
        document = new Document();

        Stream stream = new FileStream(filePath, FileMode.Create);
        pdfWriter = PdfWriter.GetInstance(document, stream);
        document.Open();
    }


    /// <summary>
    /// 添加标题
    /// </summary>
    /// <param name="titleContent">标题内容</param>
    /// <param name="alignment">标题对齐方式0：左对齐  1：居中对齐 2：右对齐</param>
    public void AddTitle(string titleContent, int alignmentType, Font font)
    {
        Paragraph contentP = new Paragraph(new Chunk(titleContent, font));
        contentP.Alignment = alignmentType;
        document.Add(contentP);
    }


    public void AddImage(Image image)
    {
        document.Add(image);
    }

    /// <summary>
    /// 获取一个字体
    /// </summary>
    /// <param name="fontPath">字体路径</param>
    /// <param name="fontSize"></param>
    /// <param name="fontStyle">字体风格：可以在Font类里面查看所有风格,例如粗体 Font.BOLD</param>
    /// <param name="r">（0~255）</param>
    /// <param name="g">（0~255）</param>
    /// <param name="b">（0~255）</param>
    /// <returns>返回Font  方便进行字体的一些特殊设置，所以直接返回该类型</returns>
    public Font GetFont(string fontPath, int fontSize=14, int fontStyle= Font.NORMAL, int r = 0, int g = 0, int b = 0,int a=255)
    {
        BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        BaseColor baseColor = new BaseColor(r, g, b, 255);
        Font font = new Font(baseFont, fontSize, fontStyle, baseColor);
        return font;
    }

    /// <summary>
    /// 获取一张图片
    /// </summary>
    /// <param name="imagePath"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns>返回Image的实例，方便后续进行一些特殊设置</returns>
    public Image GetImage(string imagePath, float width, float height = 0) {
        Image image = Image.GetInstance(imagePath);

        if (height != 0)
        {
            image.ScaleAbsoluteWidth(width);
            image.ScaleAbsoluteHeight(height);
        }
        else
        {
            image.ScaleToFit(width, height);
          
        }

        return image;
    }


    /// <summary>
    /// 返回一个语句块
    /// </summary>
    /// <param name="textContent"></param>
    /// <param name="font"></param>
    /// <returns></returns>
    public Chunk GetChunk(string textContent,Font font) {
        Chunk chunk = new Chunk(textContent, font);

        return chunk;
    }

    /// <summary>
    /// 返回一个短语
    /// </summary>
    /// <returns></returns>
    public Phrase GetPhrase(string textContent, Font font) {
        Phrase phrase = new Phrase(textContent, font);

        return phrase;
    }

    public Paragraph GetParagraph(string textContent, Font font) {
        Paragraph phrase = new Paragraph(textContent, font);
        return phrase;
    }


    public List GetList(List<string> contentList,Font font) {
        List list = new List(List.UNORDERED, 10f);
       

        for (int i = 0; i < contentList.Count; i++)
        {
            ListItem listItem = new ListItem(contentList[i], font);
            list.Add(listItem);
        }

        return list;
    }


    /// <summary>
    /// 创建一个表格
    /// </summary>
    /// <param name="col">表格列数</param>
    /// <returns></returns>
    public PdfPTable GetPdfPTable(int col) {

        PdfPTable table = new PdfPTable(6);//为pdfpTable的构造函数传入整数3，pdfpTable被初始化为一个三列的表格
       

        return table;
    }

    public PdfPCell GetPdfPCell(string Content,Font font) {
        PdfPCell cell = new PdfPCell(GetPhrase(Content,font));

        return cell;
    }

    /// <summary>
    /// 添加倾斜水印
    /// </summary>
    /// <param name="inputfilepath"></param>
    /// <param name="outputfilepath"></param>
    /// <param name="waterMarkName"></param>
    /// <param name="userPassWord"></param>
    /// <param name="ownerPassWord"></param>
    /// <param name="permission"></param>
    public  void setWatermark(string inputfilepath, string outputfilepath, string waterMarkName,Font font)
    {
        PdfReader pdfReader = null;
        PdfStamper pdfStamper = null;
        try
        {
            pdfReader = new PdfReader(inputfilepath);
            pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
            // 设置密码 
            //pdfStamper.SetEncryption(false,userPassWord, ownerPassWord, permission);

            int total = pdfReader.NumberOfPages + 1;
            PdfContentByte content;
           
            PdfGState gs = new PdfGState();
            gs.FillOpacity = 0.8f;//透明度

            int j = waterMarkName.Length;
            char c;
            int rise = 0;
            for (int i = 1; i < total; i++)

            {
                rise = 500;
                content = pdfStamper.GetOverContent(i);//在内容上方加水印
                 //content = pdfStamper.GetUnderContent(i);//在内容下方加水印                                     

                content.BeginText();
                content.SetColorFill(BaseColor.DARK_GRAY);
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                
                content.SetFontAndSize(baseFont, 20);
                // 设置水印文字字体倾斜 开始
                if (j >= 15)
                {
                    content.SetTextMatrix(200, 120);
                    for (int k = 0; k < j; k++)
                    {
                        content.SetTextRise(rise);
                        c = waterMarkName[k];
                        content.ShowText(c + "");
                        rise -= 20;
                    }
                }
                else
                {
                    content.SetTextMatrix(180, 100);
                    for (int k = 0; k < j; k++)
                    {
                        content.SetTextRise(rise);
                        c = waterMarkName[k];
                        content.ShowText(c + "");
                        rise -= 18;
                    }
                }
                // 字体设置结束
                content.EndText();
                //// 画一个圆
                //content.Ellipse(250, 450, 350, 550);
                //content.SetLineWidth(1f);
                //content.Stroke();
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

            if (pdfStamper != null)
                pdfStamper.Close();

            if (pdfReader != null)
                pdfReader.Close();
        }
    }

    public void Dispose()
    {
        document.Close();
        pdfWriter.Close();
    }

    private void OnDisable()
    {
        document.Close();
        pdfWriter.Close();



    }

    public static IEnumerator CopyFile(string Oldpath, string newPath)
    {
        if (File.Exists(newPath))
        {
            yield break;
        }
        Uri uri = new Uri(Oldpath);
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (string.IsNullOrEmpty(request.error))
            {
                File.WriteAllBytes(newPath, request.downloadHandler.data);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }
    }
}
