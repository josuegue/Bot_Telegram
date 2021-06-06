using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Bot_Exa_Comida.Clsconexionmysql;
using System.Data;
using Bot_Exa_Comida.Emoijis;

namespace Bot_Exa_Comida.Clases.Clschatbot
{
    class ClsBotTelegram
    {
        private static long userid;
        private static string nombre;
        ClsConexion ConexionBd = new ClsConexion(userid, nombre, Nombre_orden);


        private static TelegramBotClient Bot;
        private static string Nombre_orden { get; set; }

        public async Task InicioEjemploTelegram()
        {

            Bot = new TelegramBotClient("1878780445:AAEvKFBlYT__chD1cyFWrXrN1Y7bZPSIWiU");


            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Escuchando peticines de @{me.Username}");
            Console.Write("Esta diciendo:  ");

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            
            var message = messageEventArgs.Message;
            userid = message.Chat.Id;
            nombre = message.Chat.FirstName;
            

            if (message == null || message.Type != MessageType.Text)
                return;
            Console.WriteLine($"El usuario {message.Chat.Id} con nombre {message.Chat.FirstName} solicita el comando: {message.Text}");

            switch (message.Text.Split(' ').First())
            {
                // Comando 1
                case "/mostrar":
                    await SendInlineKeyboard(message);
                    break;

                // Comando 2
                case "/consulta":
                    await Boton_clta(message);
                    break;
                // Comando 3
                case "/eliminar":
                    await Boton_Eliminar(message);
                    break;

                default:
                    await Usage(message);
                    break;
            }

            // Despliega el teclado en linea
            // Se puede responder por el metodo BotOnCallbackQueryReceived
            static async Task SendInlineKeyboard(Message message)
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                // Tiempo de espera
                await Task.Delay(500);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // primera fila
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Tacos al pastor"),
                        InlineKeyboardButton.WithCallbackData("Tacos de res"),
                    },
                    // segunda fila
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Hamburguesa de res doble carne"),
                        InlineKeyboardButton.WithCallbackData("Hamburguesa de pollo "),
                    },
                    // tercera fila
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Tortas de adobado"),
                        InlineKeyboardButton.WithCallbackData("Tortas de pollo"),
                    },
                    // cuarta fila
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Gringas"),
                        InlineKeyboardButton.WithCallbackData("Torta cubana"),
                    }

                });

                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Toda las comidas incluyen bebidas." +
                    "\nSelecciona tu opcion:",
                    replyMarkup: inlineKeyboard
                );
            }

            // se generan las consultas
            static async Task Boton_clta(Message message)
            {
                ClsConexion ConexionBd = new ClsConexion(userid, nombre, Nombre_orden);
                DataTable dt = new DataTable();
                string datos_consulta = " ";
                if (true)
                {
                    ConexionBd.Consulta_pedido();
                    dt = ConexionBd.Consulta_pedido();
                    foreach (DataRow fila in dt.Rows)
                    {
                        datos_consulta ="Hola " + fila["nombre"].ToString()+" has solicitado:\n" +
                            fila["comida"].ToString()+" con bebida " + fila["bebida"].ToString()+"\n" +
                            "Con fecha " + fila["fecha_pedido"].ToString()+" y un precio de Q." + fila["precio"].ToString()+".00, \n" +
                            $"Pronto tendras tu pedido {Emoijis.ClsEmoiji.chequesito}";
                    }

                    Console.WriteLine($"El usuario {message.Chat.Id} con nombre {message.Chat.FirstName} ha consultado su pedido.");

                    await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: datos_consulta
                    );
                }    
            }
            //Proceso para hacer eliminacion
            static async Task Boton_Eliminar(Message message)
            {
                ClsConexion ConexionBd = new ClsConexion(userid, nombre, Nombre_orden);
                string eliminar_pedido = $"Tu pedido ha sido eliminado, ¡Gracias {message.Chat.FirstName} por tu preferencia!";
                if (true)
                {
                    ConexionBd.Eliminar_pedido();

                    Console.WriteLine($"Se ha eliminado el pedido de {message.Chat.Id} con nombre {message.Chat.FirstName} .");

                    await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: eliminar_pedido
                    );   
                }
            }
            //desplega el menu
            static async Task Usage(Message message)
            {
                var obj_ms = message;
                if (message.Text.ToLower().Contains("/hola"))
                {
                    string respuesta = "Hola " + obj_ms.Chat.FirstName + $" {Emoijis.ClsEmoiji.smile}"+ ".\nUsa estas palabras para interactuar con nosotros.\nPuedes precionarlas o escribirlas:\n\n" +
                                       "/mostrar   - se muestra nuestro menu delicioso y puedes seleccionar tu pedido\n" +
                                       "/consulta - consulta tu pedido y cuanto vas a pagar\n"+
                                       "/eliminar  - elimina tu pedido";

                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: respuesta,
                        replyMarkup: new ReplyKeyboardRemove()
                    );
                }
                else
                {
                    await Bot.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       text: "Escribe /hola para poder interactuar con nosotros o preciona la palabra.",
                       replyMarkup: new ReplyKeyboardRemove()
                   );
                }

            }
        }

        // Proceso de botones en linea
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            string pedido = "";
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Has seleccionado: {callbackQuery.Data}, tu pedido se esta cocinando {Emoijis.ClsEmoiji.lentes_oscuros}."

            );

            await Bot.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Has seleccionado: {callbackQuery.Data}, tu pedido se esta cocinando {Emoijis.ClsEmoiji.lentes_oscuros}."
            );
            Console.WriteLine($"El usuario ha solicitado {callbackQuery.Data}");

            // se guardan datos  de la comida 
            pedido = callbackQuery.Data.ToString();
            Nombre_orden = pedido;
            ClsConexion ConexionBd = new ClsConexion(userid, nombre, Nombre_orden);
            DataTable dt = new DataTable();
            if (true)
            {
                ConexionBd.Hacer_pedido();
                Console.WriteLine("Se ha hecho un nuevo pedido.");
            }
        }
      
        #region Inline Mode

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };
            await Bot.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryEventArgs.InlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        #endregion

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Error recibido: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }
    }
}


