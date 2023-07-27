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

            warehouse.Show(); //Вывод всех товаров на складе с их остатком

            Console.ReadLine();

            Cart cart = shop.Cart();
            cart.AddGood(iPhone12, 4);
            cart.AddGood(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            cart.ShowCart(); //Вывод всех товаров в корзине

            Console.ReadLine();

            Console.WriteLine(cart.Order().PayLink);

            Console.ReadLine();

            warehouse.Show();

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
        private Dictionary<Good, int> _goods = new Dictionary<Good, int>();
        public IReadOnlyDictionary<Good, int> Goods => _goods;

        public void Delive(Good good, int count)
        {
            bool indexCell = _goods.ContainsKey(good);

            if (indexCell == false)
                _goods.Add(good, count);
            else
                _goods[good] = Merge(good, count);
        }

        public int Merge(Good newGood, int count)
        {
            int existCount = _goods[newGood];

            return existCount + count;
        }

        public List<Good> TryGetGood(Good good, int count)
        {
            StringBuilder _notFoundText = new StringBuilder($"Product or desired quantity not found. Good: {good.Name} Count: {count}.\n");
            List<Good> goods = new List<Good>();
            bool indexCell = _goods.ContainsKey(good);

            if (indexCell != false && _goods[good] >= count)
            {
                for (int i = 0; i < count; i++)
                {
                    goods.Add(good);
                }

                _goods[good] = Merge(good, -count);

                return goods;
            }
            else
            {
                Console.WriteLine(_notFoundText);
            }

            return null;
        }

        public void Show()
        {
            StringBuilder goodText = new StringBuilder();
            Console.WriteLine("\nGoods in stock:");
            Console.WriteLine("-------------------------");

            foreach (var good in _goods)
            {
                goodText.Append($"Good:{good.Key.Name} Count:{good.Value}");
                Console.WriteLine(goodText);
                goodText.Clear();
            }

            Console.WriteLine("-------------------------");
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

            if (newGoods != null)
            {
                foreach (Good good in newGoods)
                {
                    _goods.Add(good);
                }
            }
        }

        public void ShowCart()
        {
            StringBuilder goodText = new StringBuilder();
            Console.WriteLine("Goods in the cart:");

            for (int i = 1; i <= _goods.Count; i++)
            {
                goodText.Append($"Good {i}: {_goods[i - 1].Name}");
                Console.WriteLine(goodText);
                goodText.Clear();
            }
        }

        public Shop Order() => _shop;
    }
}
