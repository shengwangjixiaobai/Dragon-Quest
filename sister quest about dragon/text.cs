using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace sister_quest_about_dragon
{
    class program
    {
        //图像的缩放
        private static Bitmap ResizeImage(Bitmap bitmap,int width,int height)
        {
            Bitmap resizedBitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(resizedBitmap);//返回一个graphics对象
            graphics.DrawImage(bitmap,0,0,width,height);//更新resizedbitmap(画布)的图像数据(将bitmap绘制到上面去)
            //drawImage函数如果是winform程序是可以直接绘图的，但是窗口程序无法使用。
            return resizedBitmap;
        } 
        
        private static void PrintAsiiArt(Bitmap resizedBitmap)
        {
            //通过字符的可视密度构建不同的灰度值
            string asciiChars = "@#8&%WM*+=-:. ";
            int numChars = asciiChars.Length;//用于灰度索引的计算
            int width = resizedBitmap.Width;
            int height = resizedBitmap.Height;
            //string characterPictures = "";
            StringBuilder sb = new StringBuilder();//stringbuilder类可以动态构建含有ansi转义码的字符串
            //构建图像字符画(bitmap是用矩阵的方式存储图片的像素)
            for(int y = 0; y < height; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    //获取像素点的颜色
                    Color pixelColor = resizedBitmap.GetPixel(x,y);
                    //计算灰度值(表示从黑色(0)到白色(255)的灰色不同程度)
                    int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                    /*
                     int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11): 计算当前像素的灰度值。
                     使用加权平均方法将 RGB 颜色转换为灰度值，其中红色、绿色和蓝色的权重分别为 0.3、0.59 和 0.11。
                     这些权重值是根据人眼对颜色敏感度的不同设定的
                     */
                    //做一个灰度值到asscii字符的映射(灰度值的范围在0到255)
                    //int asciiIndex = grayValue * numChars / 255;
                    //公式推导
                    /*
                        灰度值 / 255（灰度最大值） = 对应索引值 / 索引的最大值
                     
                     */
                    //characterPictures += asciiChars[asciiIndex];
                    //char asciichar = asciiChars[asciiIndex];
                    //得到对应的ansi转义码
                    string colorCode = $"\x1b[38;2;{pixelColor.R};{pixelColor.G};{pixelColor.B}m";
                    sb.Append(colorCode + "#");
                }

                //characterPictures += '\n';
                sb.AppendLine("\x1b[0m"); // 重置颜色
            }

            //Console.Write(characterPictures);
            Console.WriteLine(sb.ToString());//这个tostring方法是在sb类中被重写了的

        }
        
        
        static void Main(string[] args)
        {

            //设置窗口大小
            Console.SetWindowSize(100,50);
            Console.SetBufferSize(100,50);
            string imagepath = @"D:\bilbil\老八.jpg"; // 图片路径
            
            //字符画的宽度和高度
            int consoleWidth = 80;
            int consoleHeith = 30;
            //加载图像(用bitmap类来加载图片)
            Bitmap bitmap = new Bitmap(imagepath);
            Bitmap resizedBitmap = ResizeImage(bitmap,consoleWidth,consoleHeith);//缩放图像
            PrintAsiiArt(resizedBitmap);//打印图像

        }
    }


}
