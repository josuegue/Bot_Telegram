using Bot_Exa_Comida.Clases.Clschatbot;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Bot_Exa_Comida.Clsconexionmysql;
namespace Bot_Exa_Comida
{
    class Program
    {

        //static void Main(string[] args)
        //{
        //    ClsConexion con = new ClsConexion(284384585, "Josue");
        //    string pedido = con.Hacer_pedido();
        //    con.CrudMysql(pedido);
        //}
        public static async Task Main()
        {
            await new ClsBotTelegram().InicioEjemploTelegram();
        }
    }
}
