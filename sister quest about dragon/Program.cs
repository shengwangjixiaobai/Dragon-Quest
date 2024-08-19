using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Dsp;
using System.Threading;
//using System.Text;
using NAudio.Wave;
namespace Sister_Quest
{
    
    
    class program
    {

        private static Thread monitorThread;
        private static CancellationTokenSource cts;
        private static CancellationToken token;
        //游戏音乐播放器
        private static WaveOutEvent audioDevice_music = new WaveOutEvent();
        //妹妹语言播放器
        private static WaveOutEvent audioDevice_sister = new WaveOutEvent();

        private static void Start_musicThread(string bgm_path) 
        {
            //实现monitorThread;
            monitorThread = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {

                    if (audioDevice_music.PlaybackState == PlaybackState.Stopped)
                    {
                        AudioFileReader audioFile = new AudioFileReader(bgm_path);
                        audioDevice_music.Init(audioFile);
                        audioDevice_music.Play();
                    }
                }
                if (token.IsCancellationRequested)
                {
                    audioDevice_music.Stop();

                }
            });
            
            monitorThread.Start();

        }
        
        
        /// <summary>
        /// 音乐播放
        /// </summary>
        /// <param name="audio_path"></param>
        private static void Play_audio(string audio_path,WaveOutEvent outputDevice)
        {
            //音频路径
            string audioFilePath = audio_path;
            //检查文件是否存在

            if (!System.IO.File.Exists(audioFilePath))
            {
                Console.Write("音频文件丢失");
                Environment.Exit(0);

            }
            AudioFileReader audioFile = new AudioFileReader(audioFilePath);
            //outputDevice = new WaveOutEvent();
            try
            {
                outputDevice.Init(audioFile);
            }
            catch
            {
                outputDevice.Stop();
                //outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
            }
            outputDevice.Play(); //异步播放

        }
        //图像的缩放
        private static Bitmap ResizeImage(Bitmap bitmap, int width, int height)
        {
            Bitmap resizedBitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(resizedBitmap);//返回一个graphics对象
            graphics.DrawImage(bitmap, 0, 0, width, height);//更新resizedbitmap(画布)的图像数据(将bitmap绘制到上面去)
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
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    //获取像素点的颜色
                    Color pixelColor = resizedBitmap.GetPixel(x, y);
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
            Console.Write(sb.ToString());//这个tostring方法是在sb类中被重写了的

        }
        private static void PrintAsiiArt_game(Bitmap resizedBitmap)
        {
            int width = resizedBitmap.Width;
            int height = resizedBitmap.Height;
            //string characterPictures = "";
            StringBuilder sb = new StringBuilder();//stringbuilder类可以动态构建含有ansi转义码的字符串
            //构建图像字符画(bitmap是用矩阵的方式存储图片的像素)
            sb.Append(string.Concat(Enumerable.Repeat(" ", 50)));
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    //获取像素点的颜色
                    Color pixelColor = resizedBitmap.GetPixel(x, y);
                    string colorCode = $"\x1b[38;2;{pixelColor.R};{pixelColor.G};{pixelColor.B}m";
                    sb.Append(colorCode + "#");
                }

                //characterPictures += '\n';
                if (y != height - 1)
                {
                    sb.Append("\x1b[0m" + "\n" + string.Concat(Enumerable.Repeat(" ", 50))); // 重置颜色
                }
            }

            //Console.Write(characterPictures);
            Console.Write(sb.ToString());//这个tostring方法是在sb类中被重写了的

        }

        //Boss类
        struct Boss
        {
            public int x;
            public int y;
            public int hp;
            public int AttackMin;
            public int AttackMax;
            public string BossIcon;
            public ConsoleColor BossColor;
        }
        //玩家类
        struct Player
        {
            public int x;
            public int y;
            public int hp;
            public int AttackMin;
            public int AttackMax;
            public string PlayerIcon;
            public ConsoleColor PlayerColor;


        }
        static void Main(string[] args)
        {
            
            #region 设置控制台
            //隐藏光标
            Console.CursorVisible = false;
            //设置窗口大小
            int w = 50, h = 30;
            Console.SetWindowSize(w, h);
            Console.SetBufferSize(w,h);
            #endregion
            //给每个场景一个编码
            int ScreenId = 0;
            int end_id = 0;//结局
            while (true)
            {
                switch (ScreenId)
                {
                    //开始场景
                    case 0:
                        #region 开始界面
                        cts = new CancellationTokenSource();
                        token = cts.Token;
                        string bgm_front = @".\火车叨位去.wav";
                        Start_musicThread(bgm_front);
                        Console.Clear();
                        Console.SetWindowSize(w, h);
                        Console.SetBufferSize(w, h);
                        Console.SetCursorPosition(w / 2 - 10, 7);
                        Console.Write("勇者斗恶龙之拯救一抹多");
                        int Selindex = 0;  //游戏开关选择
                        bool is_SwitchScreen = false;
                        while (true)
                        {
                            Console.ForegroundColor = Selindex == 0 ? ConsoleColor.Red : ConsoleColor.White;
                            Console.SetCursorPosition(w / 2 - 5, 11);
                            Console.Write("开始游戏");
                            Console.ForegroundColor = Selindex == 1 ? ConsoleColor.Red : ConsoleColor.White;
                            Console.SetCursorPosition(w / 2 - 5, 14);
                            Console.Write("退出游戏");
                            //选择
                            char Sel_Press = Console.ReadKey(true).KeyChar;
                            switch (Sel_Press)
                            {
                                case 'w':
                                    //判断一下是否可以往上进行选择
                                    if (Selindex - 1 >= 0)
                                    {
                                        Selindex -= 1;

                                    }
                                    break;


                                case 's':
                                    if (Selindex + 1 <= 1)
                                    {
                                        Selindex += 1;
                                    }
                                    break;
                                //切换场景 
                                case 'j':
                                    if (Selindex == 0)
                                    {
                                        is_SwitchScreen = true;
                                        ScreenId = 1;
                                    }
                                    else
                                    {
                                        Play_audio(@".\fem_asu_00199.mp3", audioDevice_sister);
                                        Thread.Sleep(3000);
                                        Environment.Exit(0);
                                        //return;
                                    }
                                    break;
                            }
                            //选择结束后判断一下是否跳转了场景
                            if (is_SwitchScreen)
                            {
                                Console.Clear();
                                cts.Cancel();
                                monitorThread.Join();
                                break;
                            }

                        }
                        #endregion

                        break;

                    //游戏场景(gamestart)
                    case 1:
                        #region 游戏窗口设置
                        Console.Clear();
                        //设置窗口大小
                        Console.SetWindowSize(150, 55);
                        Console.SetBufferSize(150, 55);
                        string imagepath1 = @".\ev_com_01.png"; // 图片路径                                                                                                                                     //字符画的宽度和高度
                        int consoleWidth1 = 100;
                        int consoleHeith1 = 55;
                        //加载图像(用bitmap类来加载图片)
                        try
                        {
                            Bitmap bitmap = new Bitmap(imagepath1);
                            Bitmap resizedBitmap = ResizeImage(bitmap, consoleWidth1, consoleHeith1);//缩放图像
                            Console.SetCursorPosition(0, 0);
                            PrintAsiiArt_game(resizedBitmap);//打印图像

                        }
                        catch
                        {
                            Console.Write("图片丢失");
                            Environment.Exit(0);

                        }

                        #endregion
                        #region 加载边界
                        //打印横着的墙壁
                        Console.ForegroundColor = ConsoleColor.Red;//设置墙体颜色
                        for (int i = 0; i < w ; i += 2)
                        {
                            Console.SetCursorPosition(i, 0);
                            Console.Write("■");
                            Console.SetCursorPosition(i,h/2 + 7 );
                            Console.Write("■");
                            Console.SetCursorPosition(i,h-1);
                            Console.Write("■");

                        }
                        
                        //打印竖着的墙壁
                        for (int i = 0; i < h; ++i)
                        {
                            Console.SetCursorPosition(0, i);
                            Console.Write("■");
                            Console.SetCursorPosition(w-2, i);
                            Console.Write("■");
                        }

                        #endregion
                        
                        #region Boss设置
                        //初始化boss1
                        Boss boss1;
                        boss1.x = 24;
                        boss1.y = 18;
                        boss1.hp = 100;
                        boss1.AttackMin = 10;
                        boss1.AttackMax = 20;
                        boss1.BossIcon = "■";
                        boss1.BossColor = ConsoleColor.Green;
                        /*
                        Console.SetCursorPosition(boss1.x, boss1.y);
                        Console.ForegroundColor = boss1.BossColor;
                        Console.Write(boss1.BossIcon);
                        */
                        #endregion
                        
                        #region 玩家设置
                        Player player;
                        player.x = 4;
                        player.y = 3;
                        player.hp = 100;
                        player.AttackMin = 11;
                        player.AttackMax = 22;
                        player.PlayerIcon = "●";
                        player.PlayerColor = ConsoleColor.Yellow;

                        #endregion

                        //游戏背景讲述
                        Console.SetCursorPosition(w / 2 - 14, h + 3);
                        string story = "在一个暗黑的王国里，\n" +
                                 "      传说中最强的恶龙绑架了勇者艾伦的妹妹.\n" +
                                 "      得知这个噩耗，艾伦的眼神变得坚毅如铁。\n" +
                                 "      他誓言要用自己无敌的力量拯救妹妹.";

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(story);
                        Thread.Sleep(1000);
                        Play_audio(@".\feiai_stroy_front.wav", audioDevice_sister);
                        Thread.Sleep(13000);
                        string bgm_path = @".\bgm.mp3";
                        cts = new CancellationTokenSource();
                        token = cts.Token;
                        Start_musicThread(bgm_path);
                        
                        Console.SetCursorPosition(2, h / 2 + 8);
                        Console.Write("游戏开始");
                        #region  游戏过程
                        //移动判断
                        string move = "wasd";
                        bool is_fight = false;
                        Random random = new Random();
                        //妹妹的位置
                        int sis_x = 10;
                        int sis_y = 5;
                        bool is_win = false; //胜利的标志
                        
                        
                        while (true)
                        {
                           
                            #region boss属性设置
                            if (boss1.hp > 0)
                            {
                                Console.SetCursorPosition(boss1.x, boss1.y);
                                Console.ForegroundColor = boss1.BossColor;
                                Console.Write(boss1.BossIcon);
                            }
                            else
                            {
                                //清楚boss
                                Console.SetCursorPosition(boss1.x, boss1.y);
                                Console.Write("  ");
                            }

                            #endregion

                            #region 玩家位置状态  
                            if (player.hp > 0)
                            {
                                Console.ForegroundColor = player.PlayerColor;
                                Console.SetCursorPosition(player.x, player.y);
                                Console.Write(player.PlayerIcon);
                            }

                            #endregion
                            
                            char Playerpress = Console.ReadKey(true).KeyChar;

                            if (is_fight)
                            {
                                //玩家进入战斗状态

                                if (Playerpress == 'j')
                                {
                                    //检测玩家是否死亡
                                    if (player.hp <= 0)
                                    {
                                        
                                        ScreenId = 2;
                                        audioDevice_music.Stop();
                                        cts.Cancel();
                                        monitorThread.Join();
                                        
                                        break;

                                    }
                                    else if (boss1.hp <= 0)
                                    {
                                        is_fight = false;
                                        
                                        //设置妹妹
                                        Console.SetCursorPosition(sis_x, sis_y);
                                        Console.Write("▲(妹妹)");                                   
                                    }

                                    if (is_fight)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.SetCursorPosition(2, h / 2 + 12);
                                        Console.Write("哥  哥， 加    油！！！");
                                        //玩家打boss
                                        int atk_player = random.Next(player.AttackMin, player.AttackMax + 1);
                                        boss1.hp -= atk_player;

                                        //清楚上一次的打印信息
                                        Console.SetCursorPosition(2, h / 2 + 9);
                                        Console.Write(string.Concat(Enumerable.Repeat(" ", 40)));
                                        //打印新的信息
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.SetCursorPosition(2, h / 2 + 9);
                                        Console.Write($"你对boss造成了{atk_player}点伤害,boss剩余{boss1.hp}点hp");


                                        //boss打玩家
                                        if (boss1.hp > 0)
                                        {
                                            int atk_boss = random.Next(boss1.AttackMin,boss1.AttackMax + 1);
                                            //zint atk_boss = 100;
                                            player.hp -= atk_boss;

                                            //清楚上一次的打印信息
                                            Console.SetCursorPosition(2, h / 2 + 10);
                                            Console.Write(string.Concat(Enumerable.Repeat(" ",40)));
                                            //打印新的信息
                                            if (player.hp > 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                                Console.SetCursorPosition(2,h / 2 + 10);
                                                Console.Write($"boss对你造成了{atk_boss}点伤害,你剩余{player.hp}");                                     
                                                

                                            }
                                            else
                                            {
                                                //游戏失败
                                                Console.SetCursorPosition(2, h / 2 + 8);
                                                Console.Write(string.Concat(Enumerable.Repeat(" ", 40)));
                                                Console.SetCursorPosition(2, h / 2 + 9);
                                                Console.Write(string.Concat(Enumerable.Repeat(" ", 40)));
                                                Console.SetCursorPosition(2, h / 2 + 10);
                                                Console.Write(string.Concat(Enumerable.Repeat(" ", 40)));
                                                Console.SetCursorPosition(2, h / 2 + 8);
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.Write("你已经死亡，按下j键继续...");

                                            }

                                        }
                                        else
                                        {

                                            //打印胜利消息                                                                                     
                                            Play_audio(@".\feiai.wav",audioDevice_sister);
                                            Console.SetCursorPosition(2, h / 2 + 8);
                                            Console.Write(string.Concat(Enumerable.Repeat(" ", 36)));
                                            Console.SetCursorPosition(2, h / 2 + 9);
                                            Console.Write(string.Concat(Enumerable.Repeat(" ", 40)));
                                            Console.SetCursorPosition(2, h / 2 + 10);
                                            Console.Write(string.Concat(Enumerable.Repeat(" ", 40)));
                                            Console.SetCursorPosition(2, h / 2 + 11);
                                            Console.Write(string.Concat(Enumerable.Repeat(" ", 40)));
                                            Console.SetCursorPosition(2, h / 2 + 8);
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.Write("恭喜你战胜了boss,快去营救妹妹吧");
                                            Console.SetCursorPosition(2, h / 2 + 9);
                                            Console.Write("按下j键继续...");
                                            
                                        }


                                    }

                                }                    
                            }
                            else
                            {
                                                             
                                //当玩家按下按下移动键后清楚上一个位置
                                if (move.Contains(Playerpress))
                                {
                                    Console.SetCursorPosition(player.x, player.y);
                                    Console.Write("  ");
                                };

                                //按键设置
                                switch (Playerpress)
                                {
                                    case 'w':


                                        if (
                                             player.y - 1 > 0 &&
                                             (
                                              (player.x != boss1.x || player.y - 1 != boss1.y || boss1.hp <= 0) &&
                                              (player.x < sis_x || player.x >= sis_x + 8 || player.y - 1 != sis_y || boss1.hp>0)
                                             )
                                             
                                            )

                                        {
                                            player.y -= 1;
                                        }


                                        break;

                                    case 's':
                                        if (
                                            (player.y + 1 < h / 2 + 7) &&
                                            (
                                             (player.x != boss1.x || player.y + 1 != boss1.y || boss1.hp <= 0) &&
                                             (player.x < sis_x || player.x >= sis_x + 8 || player.y + 1 != sis_y || boss1.hp > 0)
                                            )
                                           )
                                        {
                                            player.y += 1;

                                        }

                                        break;

                                    case 'a':
                                        if (
                                            (player.x - 2 > 0) &&
                                            (
                                             (player.x - 2 != boss1.x || player.y != boss1.y || boss1.hp <= 0)&&
                                             (player.x  < sis_x || player.x - 2 >= sis_x + 8 || player.y  != sis_y || boss1.hp > 0)
                                            )
                                           )
                                        {
                                            player.x -= 2;

                                        }
                                        break;

                                    case 'd':
                                        if (
                                            (player.x + 2 < w - 2) &&
                                            (
                                             (player.x + 2 != boss1.x || player.y != boss1.y || boss1.hp <= 0)&&
                                             (player.x + 2 < sis_x || player.x >= sis_x + 8 || player.y  != sis_y || boss1.hp > 0)
                                            )
                                          )
                                        {
                                            player.x += 2;

                                        }
                                        break;
                                    case 'j':
                                        
                                        if (
                                            (player.x == boss1.x - 2 && player.y == boss1.y ||
                                            player.x == boss1.x + 2 && player.y == boss1.y ||
                                            player.x == boss1.x && player.y == boss1.y - 1 ||
                                            player.x == boss1.x && player.y == boss1.y + 1   )&&
                                            boss1.hp > 0
                                           )

                                        {
                                            //进入战斗状态
                                            is_fight = true;
                                            Console.SetCursorPosition(2, h / 2 + 8);
                                            Console.Write("战斗开始");
                                            //Play_audio(@"", audioDevice_music);

                                        }
                                        if (
                                            (player.x == sis_x - 2 && player.y == sis_y ||
                                            player.x == sis_x + 8 && player.y == sis_y ||
                                            player.x > sis_x - 2 && player.x <= sis_x + 8 && player.y == sis_y - 1 ||
                                            player.x > sis_x - 2 && player.x <= sis_x + 8 && player.y == sis_y + 1) &&
                                            boss1.hp <= 0
                                          
                                           )
                                        {
                                            //进入结束场景
                                            is_win = true;
                                            ScreenId = 2;
                                            end_id = 1;
                                        }    
                                            
                                        break;

                                }

                                if (is_win)
                                {
                                    audioDevice_music.Stop();
                                    cts.Cancel();
                                    monitorThread.Join();
                                    break;
                                }
                            }  
                                               
                        }

                        #endregion
                        
                        break;
                                      
                    //结束场景(gameover)
                    case 2:
                        Console.Clear();
                        if(end_id == 1)
                        {
                            //胜利结局
                            Play_audio(@".\audio (2).wav", audioDevice_sister);
                            Play_audio(@".\甲田雅人,浜口史郎 - 英雄の証.mp3", audioDevice_music);
                            //设置窗口大小
                            Console.SetWindowSize(100, 65);
                            Console.SetBufferSize(100, 65);
                            string imagepath = @".\ev_com_01c2.jpg";// 图片路径
                            //字符画的宽度和高度
                            int consoleWidth = 100;
                            int consoleHeith = 50;
                            //加载图像(用bitmap类来加载图片)
                            try
                            {
                                Bitmap bitmap = new Bitmap(imagepath);
                                Bitmap resizedBitmap = ResizeImage(bitmap, consoleWidth, consoleHeith);//缩放图像
                                Console.SetCursorPosition(0, 0);
                                PrintAsiiArt(resizedBitmap);//打印图像

                            }
                            catch
                            {
                                Console.Write("图片丢失");
                                Environment.Exit(0);

                            }
                            
                            Console.Write("恭            喜            你               救            出             妹                妹");

                            Thread.Sleep(7000);
                            Console.SetCursorPosition(0, 50+4);


                            string story1 = "经过一场惊天动地的战斗，\n" +
                                            "艾伦凭借无尽的毅力和勇气，将恶龙彻底击败。\n" +
                                            "火山爆发的余波中，\n" +
                                             "他找到了被困在黑暗囚笼中的莉莉，\n" +
                                              "解开了她的束缚。\n" +
                                    "妹妹眼中泛起泪光，她微笑着说：\n" +
                                    "    “哥哥，你果然没有让我失望。”\n" +
                                    "艾伦将她抱在怀里，坚定地回答：\n" +
                                    "    “妹妹，别怕，我会永远保护你。”";
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(story1);
                            
                            Play_audio(@".\game_end.wav", audioDevice_sister);
                            Thread.Sleep(32000);
                            int player_sec = 0;
                            bool is_trun = false;
                            while (true)
                            {
                                Console.SetCursorPosition(100 / 2 + 10, 50 + 4);
                                Console.ForegroundColor = player_sec == 0 ? ConsoleColor.Red : ConsoleColor.White;
                                Console.Write("回到开始界面");
                                Console.SetCursorPosition(100 / 2 + 10, 50 + 6);
                                Console.ForegroundColor = player_sec == 1 ? ConsoleColor.Red : ConsoleColor.White;
                                Console.Write("退出游戏");
                                char stress = Console.ReadKey(true).KeyChar;
                                switch (stress)
                                {
                                    case 'w':
                                        if (player_sec - 1 >= 0) player_sec--;
                                        break;


                                    case 's':
                                        if (player_sec + 1 <= 1) player_sec++;
                                        break;

                                    case 'j':
                                        if(player_sec == 0)
                                        {
                                            ScreenId = 0;
                                            is_trun = true;
                                           
                                            Console.Clear();
                                        }
                                        else if(player_sec == 1)
                                        {
                                            audioDevice_music.Stop();
                                            Play_audio(@".\fem_asu_00199.mp3", audioDevice_sister);
                                            Thread.Sleep(3000);
                                            Environment.Exit(0);
                                        }
                                        break;
                                }
                                
                                if (is_trun)
                                {
                                    audioDevice_music.Stop();
                                    break;
                                }

                            }                            

                        }
                        else
                        {

                            //失败结局
                            //Play_audio("");
                            Play_audio(@".\audio (3).wav", audioDevice_sister);
                            //设置窗口大小
                            Console.SetWindowSize(100, 60);
                            Console.SetBufferSize(100, 60);
                            string imagepath = @"D:\bilbil\feiai(ku).jpg"; // 图片路径
                            //字符画的宽度和高度
                            int consoleWidth = 100;
                            int consoleHeith = 50;
                            //加载图像(用bitmap类来加载图片)
                            Bitmap bitmap = new Bitmap(imagepath);
                            Bitmap resizedBitmap = ResizeImage(bitmap, consoleWidth, consoleHeith);//缩放图像
                            Console.SetCursorPosition(0, 0);
                            PrintAsiiArt(resizedBitmap);//打印图像
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("妹              妹             被           抓           走           了(悲哀");
                            int player_sec = 0;
                            bool is_trun = false;
                            while (true)
                            {
                                Console.SetCursorPosition(100 / 2 - 10, 50 + 4);
                                Console.ForegroundColor = player_sec == 0 ? ConsoleColor.Red : ConsoleColor.White;
                                Console.Write("回到开始界面");
                                Console.SetCursorPosition(100 / 2 - 8, 50 + 6);
                                Console.ForegroundColor = player_sec == 1 ? ConsoleColor.Red : ConsoleColor.White;
                                Console.Write("退出游戏");
                                char stress = Console.ReadKey(true).KeyChar;
                                switch (stress)
                                {
                                    case 'w':
                                        if (player_sec - 1 >= 0) player_sec--;
                                        break;


                                    case 's':
                                        if (player_sec + 1 <= 1) player_sec++;
                                        break;

                                    case 'j':
                                        if (player_sec == 0)
                                        {
                                            ScreenId = 0;
                                            is_trun = true;
                                            Console.Clear();
                                        }
                                        else if (player_sec == 1)
                                        {
                                            Play_audio(@".\fem_asu_00199.mp3", audioDevice_sister);
                                            Thread.Sleep(3000);
                                            Environment.Exit(0);
                                        }
                                        break;
                                }

                                if (is_trun)
                                {
                                   
                                    break;
                                }
                            }

                        }

                        break;


                }


            }
            
        }

    }



}

