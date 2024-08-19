using System;
using System.Text;

namespace console_text
{
    class program
    {
        static void Main(string[] args)
        {

            //设置背景颜色
            //Console.BackgroundColor = ConsoleColor.Red;
            //Console.Clear();
            //设置字符颜色
            Console.ForegroundColor = ConsoleColor.Yellow;
            //隐藏光标
            Console.CursorVisible = false;
            //坐标位置
            int x = 0 ,y = 0;
            int Cur_bufhight = 0;//当前缓冲区的高度
            while (true)
            {
                Console.SetCursorPosition(x,y);
                Console.Write("██");
                char Player_input = Console.ReadKey(true).KeyChar;
                //按下按键后先清楚前面的方块
                Console.SetCursorPosition(x, y);
                Console.Write("  ");
                
                switch (Player_input)
                {
                    case 'd':
                        if(x +2 < Console.BufferWidth)
                        {
                            x += 2;
                        }
                        break;
                    case 'w':
                        if(y-1 >= 0)
                        {
                            y -= 1;
                        }
                       
                        break;
                    case 'a':
                        if(x-2 >= 0)
                        {
                            x -= 2;
                        }
                        break;
                    case 's':
                        if(y+1 < Console.BufferHeight)
                        {
                            y += 1;
                        }
                        break;

                }

            }
         

        }

    }

}