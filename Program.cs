using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Good iPhone12 = new Good("IPhone 12");
            Good iPhone11 = new Good("IPhone 11");

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Delive(iPhone12, 10);
            warehouse.Delive(iPhone11, 1);

            shop.ShowGoods(); //Вывод всех товаров на складе с их остатком

            Console.ReadLine();

            Cart cart = shop.Cart();
            cart.AddGood(iPhone12, 4);
            cart.AddGood(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            cart.ShowCart(); //Вывод всех товаров в корзине

            Console.ReadLine();

            Console.WriteLine(cart.Order().PayLink);

            Console.ReadLine();

            shop.ShowGoods();

            Console.ReadLine();

            cart.AddGood(iPhone12, 9);

            Console.ReadLine();
            //Ошибка, после заказа со склада убираются заказанные товары
        }
    }

    class Good
    {
        public string Name { get; private set; }

        public Good(string name)
        {
            Name = name;
        }
    }

    class Warehouse
    {
        private List<Cell> _goods = new List<Cell>();

        public IReadOnlyList<Cell> Goods => _goods;

        public void Delive(Good good, int count)
        {
            var newCell = new Cell(good, count);
            int indexCell = _goods.FindIndex(cell => cell.Good == good); 

            if(indexCell == -1)
                _goods.Add(newCell);
            else
                _goods[indexCell] = _goods[indexCell].Merge(newCell);
        }

        public List<Good> TryGetGood(Good good, int count)
        {
            List<Good> goods = new List<Good>();
            int indexCell = _goods.FindIndex(cell => cell.Good == good);

            if(indexCell != -1 && _goods[indexCell].Count >= count)
            {
                for (int i = 0; i < count; i++)
                {
                    goods.Add(_goods[indexCell].Good);
                }

                _goods[indexCell] = _goods[indexCell].Merge(new Cell(good, -count));

                return goods;
            }
            else
            {
                Console.WriteLine($"Product or desired quantity not found. Good: {good.Name} Count: {count}.\n");
            }

            return null;
        }
    }

    class Cell
    {
        public Good Good { get; private set; }
        public int Count { get; private set; }

        public Cell(Good good, int count)
        {
            Good = good;
            Count = count;
        }

        public Cell Merge(Cell newCell)
        {
            if (Good != newCell.Good)
                Console.WriteLine("Error");

            return new Cell(Good, Count + newCell.Count);
        }
    }

    class Shop
    {
        private Warehouse _warehouse;
        private Cart _cart;
        
        public string PayLink { get; private set; }

        public Shop(Warehouse warehouse)
        {
            _warehouse = warehouse;
            PayLink = "Payment Success!";
        }

        public void ShowGoods()
        {
            Console.WriteLine("\nGoods in stock:");
            Console.WriteLine("-------------------------");

            foreach(Cell cell in _warehouse.Goods)
            {
                Console.WriteLine($"Good:{cell.Good.Name} Count:{cell.Count}");
            }

            Console.WriteLine("-------------------------");
        }

        public Cart Cart()
        {
            _cart = new Cart(this);

            return _cart;
        }

        public List<Good> TryGetGoods(Good good, int count)
        {
            return _warehouse.TryGetGood(good, count);
        }
    }

    class Cart
    {
        private Shop _shop;
        private List<Good> _goods = new List<Good>();

        public Cart(Shop shop)
        {
            _shop = shop;
        }

        public void AddGood(Good newGood, int count)
        {
            List<Good> newGoods = _shop.TryGetGoods(newGood, count);

            if(newGoods != null)
            {
                foreach (Good good in newGoods)
                {
                    _goods.Add(good);
                }
            }     
        }

        public void ShowCart()
        {
            Console.WriteLine("Goods in the cart:");

            for(int i = 1; i <= _goods.Count; i++)
            {
                Console.WriteLine($"Good {i}: {_goods[i - 1].Name}");
            }
        }
        
        public Shop Order() => _shop;
    }
}
