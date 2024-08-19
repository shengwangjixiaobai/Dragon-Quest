using System;
#region 输入输出
//输出
//Console.WriteLine("dadafgg"); // 换行输出
//Console.Write("dafag\n"); //不换行输出
//输入
//string a = Console.ReadLine();//读一行，直到按下回车结束,返回输入的值
//char b = (char)Console.Read(); // 读一个字母，即使输入了一个字符串，也只读首字母,返回int型;
//char c = Console.ReadKey(true).KeyChar;//检测是否按下按键,当按下按键时结束,返回对象，用keychar获取返回值
//keychar就是这个对象的一个属性，参数true让其不输出输入的值
//Console.WriteLine(a + "\n" + b + "\n" + c);

#endregion

#region 控制台的其他方法
//1.清空
Console.Clear();
//2.设置控制台大小
Console.SetWindowSize(100, 50); //设置窗口大小 
Console.SetBufferSize(1000, 500);
//3.设置光标位置
//视觉上，高是宽的二倍
Console.SetCursorPosition(20, 10);
Console.ReadKey();
Console.WriteLine();
//4.设置颜色相关
//4).设置每个字符颜色(前景颜色，foreground)
Console.ForegroundColor = ConsoleColor.Red; //Consolecolor是一个枚举
Console.WriteLine("dafgggad");
///5).设置打印背景颜色
Console.BackgroundColor = ConsoleColor.White;
Console.WriteLine("dafgggad");
Console.Clear(); //设置背景颜色后,只会在新刷新的区域有用，所以需要将之前的背景颜色清楚掉后才可以改变全屏背景颜色
                 //6).光标的显隐
Console.CursorVisible = true;//true为显示，false为隐藏
                             //7).关闭控制台
//Environment.Exit(0);//参数是返回操作系统的退出代码，0表示处理成功完成(有点类似c++main中的return 0)
//退出代码是0一般表示正常退出

#endregion