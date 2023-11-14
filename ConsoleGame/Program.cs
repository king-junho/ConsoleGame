using System;
using System.Reflection;
using System.Security.Cryptography;

namespace ConsoleGame
{
    public class Character
    {
        public string Name { get; }
        public string Job { get; }      //enum으로 구현 가능
        public int Level { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Hp { get; }
        public int Gold { get; set; }
        public int ChangeAtk { get; set; }
       
        public int ChangeDef {  get; set; }
        public int ChangeHp {  get; set; } 


        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
            ChangeAtk = 0;
            ChangeDef = 0;
            ChangeHp = 0;
        }
    }
    public class Item
    {
        public string Name { get; }
        public string Description { get; }
        public int Type { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Hp { get; }
        public int Price { get; }
        public bool IsEquipped { get; set; }
        public bool IsPurchased { get; set; }
        public Item(string name, string description, int type, int atk, int def, int hp,int price=0, bool isEquipped = false, bool isPurchased=false)
        {
            Name = name;
            Description = description;
            Type = type;
            Atk = atk;
            Def = def;
            Hp = hp;
            IsEquipped = isEquipped;
            IsPurchased = isPurchased;
            Price = price;
        }
        public void PrintItemStatDescription(bool withNumber=false,int idx=0, int shopType=0)
        {
            if(withNumber)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("{0}", idx);
                Console.ResetColor();
            }
            Console.Write("- ");
            if(IsEquipped)      //선택 됐으면
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("E");
                Console.ResetColor();
                Console.Write("]");
                Console.Write($"{PadRightForMixedText(Name, 13)}");
            }
            else Console.Write($"{PadRightForMixedText(Name, 16)}");
            Console.Write(" | ");
            
            if (Atk != 0) Console.Write($"{PadRightForMixedText($"Atk {(Atk >= 0 ? "+" : "")}{Atk}", 7)}");
            if (Def != 0) Console.Write($"{PadRightForMixedText($"Def {(Def >= 0 ? "+" : "")}{Def}", 7)}");
            if (Hp != 0) Console.Write($"{PadRightForMixedText($"Hp {(Hp >= 0 ? "+" : "")}{Hp}", 7)}");
            
            if(shopType==1)
            {
                Console.Write(" | ");
                Console.Write(PadRightForMixedText(Description, 50));
                Console.Write(" | ");
                Console.WriteLine($"{(IsPurchased==true ? "구매완료": $"{Price}G")}");
            }
            else if(shopType==2)
            {
                Console.Write(" | ");
                Console.Write(PadRightForMixedText(Description, 50));
                Console.Write(" | ");
                Console.WriteLine($"{(Price!=0 ? $"{Price * 85 / 100} G" :"팔 수 없는 기본 아이템입니다.")}");
            }
            else
            {
                Console.Write(" | ");
                Console.Write(PadRightForMixedText(Description, 50));
                Console.WriteLine();

            }


        }

        public static int GetPrintableLegnth(string str)
        {
            int length = 0;
            foreach(char c in str)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                    length += 2;
                else
                    length += 1;
            }
            return length;
        }
        public static string PadRightForMixedText(string str, int totalLength)
        {
            int currentLength = GetPrintableLegnth(str);
            int padding = totalLength-currentLength;
            return str.PadRight(str.Length + padding);
        }
    }

    internal class Program
    {
        static Character player;
        static List<Item> inventory;
        static List<Item> shop;

        static void Main(string[] args)
        {
            GameDataSetting();
            PrintStartLogo();        
            StartMenu();           
        }

        private static void StartMenu()
        {
            Console.Clear();

            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다");
            Console.WriteLine("이곳에서 던전으로 돌아가기 전 활동을 할 수 있습니다");
            Console.WriteLine();
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine();

            switch(CheckValidInput(1, 3))
            {
                case 1:
                    StatusMenu();
                    break;
                case 2:
                    InventoryMenu();
                    break;
                case 3:
                    ShopMenu();
                    break;
            }
        }

        private static void ShopMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상점");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            PrintTextWithHighlights($"{player.Gold} ", "G");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            foreach(Item item in shop)
            {
                item.PrintItemStatDescription(false,0,1);
            }
            Console.WriteLine();
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine("0. 나가기");
            Console.WriteLine();

            switch (CheckValidInput(0,2))
            {
                case 0:
                    StartMenu();
                    break;
                case 1:
                    BuyMenu();
                    break;
                case 2:
                    SellMenu();
                    break;
            }

        }

        private static void SellMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상점 - 아이템 판매");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 판매할 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            PrintTextWithHighlights($"{player.Gold} ", "G");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            int index = 1;
            foreach(Item item in inventory)
            {
                item.PrintItemStatDescription(true, index++,2);
            }

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();

            int inputType = CheckValidInput(0, inventory.Count);
            switch(inputType)
            {
                case 0:
                    ShopMenu();
                    break;
                default:
                    //player의 useGold 값을 빼주고 즉 돌려주고
                    //해당 index의 장비가 장착되어 있었다면 장착 해제 해줌과 동시에 능력치도 반환
                    //해당 index의 장비가
                    //구매 기록도 반환
                    //inventory에도 삭제
                    inventory[inputType - 1].IsPurchased = false;
                    //player.UseGold = (player.Gold-player.UseGold)+(inventory[inputType - 1].Price)*15/100;
                    player.Gold += inventory[inputType - 1].Price * 85 / 100;
                    if (inventory[inputType - 1].IsEquipped == true)
                        ToggleEquipStatus(inputType-1);
                    inventory.Remove(inventory[inputType - 1]);
                    SellMenu();
                    break;
            }
        }

        private static void BuyMenu(int IsBuy = 0)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상점 - 아이템 구매");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            PrintTextWithHighlights($"{player.Gold} ", "G");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            int index = 1;
            foreach (Item item in shop)
            {
                item.PrintItemStatDescription(true, index++,1);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("0. ");
            Console.ResetColor();
            Console.WriteLine("나가기");
            Console.WriteLine();
            if(IsBuy==1)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{inventory.Last().Name} 구매를 완료했습니다!");
                Console.ResetColor();
                Console.WriteLine();
            }
            else if(IsBuy==2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Gold가 부족합니다.");
                Console.ResetColor();
                Console.WriteLine();
            }
            else if(IsBuy==3)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("이미 구매한 아이템입니다.");
                Console.ResetColor();
                Console.WriteLine();
            }
            int inputType = CheckValidInput(0, shop.Count);
            switch (inputType) 
            {
                case 0:
                    ShopMenu();
                    break;
                default:
                    //번호 입력하면 들어와야 처리해주는 로직 구현
                    //(1) 구매 했는지 파악하기  => item class 내 IsPurchased 둠
                    //(2) 구매 안 했으면 -> 구매 , 구매 했으면 구매했다고 출력
                    //(3) 구매 가능하면 금액 파악하고 금액 감소 부족하다면 부족 출력
                    if (shop[inputType-1].IsPurchased==false) //구매 안 했다면
                    {
                        if (player.Gold >= shop[inputType-1].Price) //플레이어 돈이 더 많다면
                        {
                            //아이템 샀다고 표시해주고
                            //player가격 내려야돼
                            //inventory에 추가
                            shop[inputType - 1].IsPurchased = true;
                            player.Gold -= shop[inputType - 1].Price;
                            inventory.Add(shop[inputType - 1]);
                            BuyMenu(1);
                        }
                        else 
                            BuyMenu(2);
                    }
                    else  //구매 됐다면
                        BuyMenu(3);
                    
                    break;
            }
        }

        private static void InventoryMenu()
        {
            Console.Clear();
            ShowHighlightedText("■ 인벤토리 ■");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다");
            Console.WriteLine("[아이템 목록]");

            foreach(Item item in inventory)
            {
                item.PrintItemStatDescription();
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착관리");
            Console.WriteLine("2. 아이템 정렬");
            Console.WriteLine();

            switch (CheckValidInput(0,2))
            {
                case 0:
                    StartMenu();
                    break;
                case 1:
                    EquipMenu();
                    break;
                case 2:
                    //아이템 정렬하고
                    //그다음에 다시 InventoryMenu() 호출
                    SortMenu();
                    break;
            }
        }

        private static void SortMenu()
        {
            Console.Clear();
            ShowHighlightedText("■ 인벤토리 - 아이템 정렬 ■");
            Console.WriteLine("보유 중인 아이템을 정렬할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            int index = 1;
            foreach (Item item in inventory)
            {
                item.PrintItemStatDescription(true, index++);
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 이름순");
            Console.WriteLine("2. 장착순");
            Console.WriteLine("3. 공격력");
            Console.WriteLine("4. 방어력");
            Console.WriteLine();

            switch (CheckValidInput(0,4))
            {
                case 0:
                    InventoryMenu();
                    break;
                case 1:
                    inventory = inventory.OrderBy(x => x.Name).ToList();
                    break;
                case 2:
                    inventory=inventory.OrderBy(x=>x.IsEquipped).Reverse().ToList();
                    break;
                case 3:
                    inventory = inventory.OrderBy(x => x.Atk).Reverse().ToList();
                    break;
                case 4:
                    inventory = inventory.OrderBy(x => x.Def).Reverse().ToList();
                    break;
            }
            SortMenu();
        }

        private static void EquipMenu()
        {
            int index = 1;

            Console.Clear();
            ShowHighlightedText("■ 인벤토리 - 장착 관리 ■");
            Console.WriteLine("보유중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");
            foreach(Item item in inventory)
            {
                item.PrintItemStatDescription(true, index++);
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();

            int keyInput = CheckValidInput(0, inventory.Count);
            
            switch(keyInput)
            {
                case 0:
                    InventoryMenu();
                    break;
                default:
                    ToggleEquipStatus(keyInput - 1);
                    EquipMenu();
                    break;
            }
        }

        private static void ToggleEquipStatus(int idx)
        {
            int flag = 1;

            if (inventory[idx].IsEquipped==false)
            {
                inventory[idx].IsEquipped = true;
            }
            else
            {
                inventory[idx].IsEquipped = false;
                flag *= -1;
            }
            player.ChangeAtk += inventory[idx].Atk*flag;
            player.ChangeDef += inventory[idx].Def*flag;
            player.ChangeHp+= inventory[idx].Hp * flag;
        }

        private static void StatusMenu()
        {
            Console.Clear();

            ShowHighlightedText("■ 상태 보기 ■");
            Console.WriteLine("캐릭터의 정보가 표기됩니다");
            PrintTextWithHighlights("Lv. ", $"{player.Level.ToString("00")}");
            Console.WriteLine();
            Console.WriteLine($"{player.Name} ( {player.Job} )");
            PrintTextWithHighlights("공격력 : ", $"{player.Atk+player.ChangeAtk}", player.ChangeAtk!=0 ? string.Format(" (+{0})",player.ChangeAtk):"");
            PrintTextWithHighlights("방어력 : ", $"{player.Def+player.ChangeDef}",player.ChangeDef!=0 ? string.Format(" (+{0})",player.ChangeDef):"");
            PrintTextWithHighlights("체 력 : ", $"{player.Hp+player.ChangeHp}",player.ChangeHp!=0 ? string.Format(" (+{0})",player.ChangeHp):"");
            PrintTextWithHighlights("Gold : ", $"{player.Gold}");
            Console.WriteLine();
            Console.WriteLine("0. 뒤로가기");
            Console.WriteLine();

            switch (CheckValidInput(0,0))
            {
                case 0:
                    StartMenu();
                    break;
            }
        }

        private static void ShowHighlightedText(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void PrintTextWithHighlights(string s1, string s2, string s3="")
        {
            Console.Write(s1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(s2);
            Console.ResetColor();
            Console.WriteLine(s3);
        }

        private static int CheckValidInput(int min, int max)
        {
            //(1) 숫자가 아닌 값을 받은 경우
            //(2) 숫자가 최소값 - 최대값을 벗어나는 경우
            int keyInput;
            bool result;
            do
            {
                Console.WriteLine("원하시는 행동을 입력해주세요. ");
                result = int.TryParse(Console.ReadLine(), out keyInput);
            } while (result == false || CheckInputValid(keyInput, min, max) == false);
            //int 변환도 문제 없고 그 값이 유효한 값인지 판별하는 while 조건
            return keyInput;
        }

        private static bool CheckInputValid(int keyInput, int min, int max)
        {
            if (min <= keyInput && keyInput <= max) return true;
            return false;
        }

        private static void PrintStartLogo()
        {
            Console.WriteLine("==============================================================");
            Console.WriteLine("________                     ____                          ");
            Console.WriteLine("\\______ \\   __ __   ____    / ___\\   ____   ____    ____   ");
            Console.WriteLine(" |    |  \\ |  |  \\ /    \\  / /_/  >_/ __ \\ /  _ \\  /    \\  ");
            Console.WriteLine(" |    `   \\|  |  /|   |  \\ \\___  / \\  ___/(  <_> )|   |  \\ ");
            Console.WriteLine("/_______  /|____/ |___|  //_____/   \\___  >\\____/ |___|  / ");
            Console.WriteLine("        \\/             \\/               \\/             \\/  ");
            Console.WriteLine("==============================================================");
            Console.WriteLine("                    PRESS ANYKEY TO START                     ");
            Console.WriteLine("==============================================================");
            Console.ReadKey();
        }

        private static void GameDataSetting()
        {
            player = new Character("chad", "전사", 1, 10, 5, 100, 1500);
            inventory = new List<Item>();
            shop = new List<Item>();

            inventory.Add(new Item("무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다", 0, 0, 5, 0));
            inventory.Add(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검입니다", 1, 2, 0, 0));
            shop.Add(new Item("수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", 0, 0, 5, 0,1000));
            shop.Add(new Item("스파르타 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 0, 0, 15, 0,3500));
            shop.Add(new Item("청동 도끼", "어디선가 사용 됐던 거 같은 도끼입니다.", 1, 5, 0, 0,1500));
            shop.Add(new Item("스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 1, 7, 0, 0,4000));        }

    }
}