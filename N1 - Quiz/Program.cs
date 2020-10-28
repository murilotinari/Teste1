using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace N1___Quiz
{
    class Program
    {
        static int qtde = System.IO.File.ReadAllLines("quiz.txt").Length;
        static string conteudo = File.ReadAllText("quiz.txt");
        static string record = File.ReadAllText("record.txt");
        static linha[] quiz = new linha[qtde];
        static int certas = 0, erradas = 0;              
        static string temaEscolhido, resposta;
        static int qtdePerguntas, primeiro, segundo, terceiro, tempoTotal, tempoPergunta, qtdeTemas, pontuacao;
        static int qtdeDados  = System.IO.File.ReadAllLines("record.txt").Length;
        static string[] dados = new string[qtdeDados];
        static int[] pontuacoes = new int[qtdeDados];
        static WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
        

        struct linha
        {
            public string tema;
            public string pergunta;
            public string A;
            public string B;
            public string C;
            public string D;
            public string certa;
        }

        ///Lê as perguntas, temas, alternativas e alternativa certa.
        static void LePerguntas()
        {

            for (int x = 0; x < qtde; x++)
            {

                quiz[x].pergunta = conteudo.Substring(0, conteudo.IndexOf('|')).Trim();
                conteudo = conteudo.Remove(0, conteudo.IndexOf('|') + 1).Trim().ToUpper();

                quiz[x].tema = conteudo.Substring(0, conteudo.IndexOf('|')).Trim();
                conteudo = conteudo.Remove(0, conteudo.IndexOf('|') + 1).Trim().ToUpper();

                quiz[x].certa = conteudo.Substring(0, conteudo.IndexOf('|'));
                conteudo = conteudo.Remove(0, conteudo.IndexOf('|') + 1).Trim().ToUpper();

                quiz[x].A = conteudo.Substring(0, conteudo.IndexOf('|'));
                conteudo = conteudo.Remove(0, conteudo.IndexOf('|') + 1).Trim().ToUpper();

                quiz[x].B = conteudo.Substring(0, conteudo.IndexOf('|'));
                conteudo = conteudo.Remove(0, conteudo.IndexOf('|') + 1).Trim().ToUpper();

                quiz[x].C = conteudo.Substring(0, conteudo.IndexOf('|'));
                conteudo = conteudo.Remove(0, conteudo.IndexOf('|') + 1).Trim().ToUpper();

                if (conteudo.IndexOf("\n") == -1)
                {
                    quiz[x].D = conteudo.Trim().ToUpper();
                }
                else
                {
                    quiz[x].D = conteudo.Substring(0, conteudo.IndexOf("\n"));
                    conteudo = conteudo.Remove(0, conteudo.IndexOf("\n")).ToUpper();
                }

            }

        }

        static void LeRecord()
        {

            record = File.ReadAllText("record.txt");
            qtdeDados = System.IO.File.ReadAllLines("record.txt").Length;
            Array.Resize(ref dados, qtdeDados);
            Array.Resize(ref pontuacoes, qtdeDados);


            if (record.Length == 0)
            {
                return;
            }
            //le record.txt
            string acumuladora = record;
            for(int x = 0; x < qtdeDados; x++)
            {
                
                dados[x] = record.Substring(0, record.IndexOf('|')+1);               
                pontuacoes[x] = int.Parse(dados[x].Substring(record.IndexOf(':') + 1, record.IndexOf('|') - record.IndexOf(':') - 2));
                record = record.Remove(0, record.IndexOf('|') + 1);
            }
            record = acumuladora;

            //calcula as 3 melhores pontuacoes
            for (int x = 0; x < qtdeDados; x++)
            {
                int quant = 0;
                for (int y = 0; y < qtdeDados;  y++)
                {
                    if (pontuacoes[x] > pontuacoes[y])
                        quant++;                     
                }

                if (quant == qtdeDados - 1)
                    primeiro = pontuacoes[x];
                else if (quant == qtdeDados - 2)
                    segundo = pontuacoes[x];
                else if (quant == qtdeDados - 3)
                    terceiro = pontuacoes[x];

            }


        }


        //metodo em que executa-se o timer ao mesmo tempo q verifica se a tecla digitada é uma alternativa
        static void timer(int exclusor)
        {
            bool verifica = false;
            Stopwatch cronometro = new Stopwatch();
            cronometro.Start();
            int tempo = 0;


            do
            {
                verifica = false;

                Console.SetCursorPosition(0, 28);
                Console.WriteLine("                   ");
                Console.SetCursorPosition(0, 28);
                Console.Write("Digite: ");

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo tecla = Console.ReadKey(true);
                    resposta = tecla.Key.ToString().ToUpper();

                    if (resposta != "A" && resposta != "B" && resposta != "C" && resposta != "D")
                    {
                        verifica = true;
                    }
                    else
                    {
                        //SE O USUARIO DIGITOU ALGUMA ALTERNATIVA O METODO IRA TESTAR SE ESTA CERTO OU ERRADO
                        avaliaResposta(exclusor);
                        tempoTotal += tempo;
                        //PARA SAIR DO METODO E CORTAR O TIMER.
                        return;
                    }
                }

                tempo = Convert.ToInt32(cronometro.Elapsed.TotalSeconds);

                //SE ELE NAO DIGITOU UMA ALTERNATIVA
                if (verifica)
                {
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("Digite apenas A, B, C ou D");
                    Console.SetCursorPosition(0, 28);
                    Thread.Sleep(1000);
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("                            ");
                }


                Console.SetCursorPosition(30, 27);
                Console.WriteLine($"Tempo: {(tempoPergunta - tempo):f1}");
                Console.SetCursorPosition(8, 28);
                Thread.Sleep(100);


            } while (tempo < tempoPergunta);

            tempoTotal += tempo;
            resposta = "errado";
            erradas++;
            

        }

        //METODO PARA AVALIAR A ALTERNATIVA ESCOLHIDA PELO USUARIO
        static void avaliaResposta(int exclusor)
        {
            if (resposta == "A")
                resposta = quiz[exclusor].A.Trim().ToUpper();
            else if (resposta == "B")
                resposta = quiz[exclusor].B.Trim().ToUpper();
            else if (resposta == "C")
                resposta = quiz[exclusor].C.Trim().ToUpper();
            else if (resposta == "D")
                resposta = quiz[exclusor].D.Trim().ToUpper();
            


            if (resposta == quiz[exclusor].certa.Trim().ToUpper())
            {
                Console.Clear();
                certas++;

            }
            else
            {
                Console.Clear();
                erradas++;
            }

        }

        
        //GERA AS PERGUNTAS ALEATORIAMENTE DE ACORDO COM O TEMA ESCOLHIDO.
        static void geraPergunta()
        {
            Random rnd = new Random();
            int[] exclusor = new int[qtdePerguntas];
            bool exclue = true;

            for (int x = 0; x < qtdePerguntas; x++)
            {
                exclue = true;

                ///sorteia um numero de pergunta
                exclusor[x] = rnd.Next(0, qtde);

                ///Para nao repetir a mesma pergunta.
                for (int y = 0; y < x; y++)
                {
                    if (exclusor[x] == exclusor[y])
                    {
                        x--;
                        exclue = false;
                    }                                                        
                }
                if (exclue == false)
                    continue;

                ///Executa o metodo para as perguntas que tem o tema escolhido
                if (quiz[exclusor[x]].tema.Trim().ToUpper() == temaEscolhido.Trim().ToUpper())
                {
                    Console.Clear();
                    Console.WriteLine($"Pergunta{x + 1}: {quiz[exclusor[x]].pergunta}");
                    Console.WriteLine($"\nA- {quiz[exclusor[x]].A}     B- {quiz[exclusor[x]].B}     C- {quiz[exclusor[x]].C}     D-{quiz[exclusor[x]].D}");
                   
                    
                    timer(exclusor[x]);
                    Console.Clear();
                    
                }
                else
                    x--;
            }
        }

        //metodo para calcular a qtde de perguntas por tema, para nao entrar em loop infinito no metodo geraPergunta
        static int qtdePerguntasTema(string temaEscolhido)
        {
            int qtdePerguntasTema = 0;
            for (int x = 0; x < qtde; x++)
            {
                if (quiz[x].tema.Trim().ToUpper() == temaEscolhido)
                    qtdePerguntasTema++;
            }

            return qtdePerguntasTema;
        }

        
        /// Metodo para calcular a qtde de temas
        static void calculaQtdeTemas()
        {
            for (int x = 0; x < qtde; x++)
            {
                for (int y = 0; y < x; y++)
                {
                    if (quiz[x].tema == quiz[y].tema)
                        continue;

                }
                qtdeTemas++;
            }
        }

        //Metodo para o usuario escolher qual tema jogar.
        static void escolheTema()
        {
            calculaQtdeTemas();
            //int escolha sera qual tema o usuario ira escolher.
            int escolha = 0;

            //bool criada para nao repetir os temas na hora de escrever
            bool exclue;

            //vetor para guardar os temas, apenas.
            string[] temas = new string[qtdeTemas];
            do
            {                

                Console.Write($"Escolha um tema para jogar: ");
                qtdeTemas = 0;

                //"for" criado para nao se escrever o mesmo tema mais de uma vez
                for(int x = 0; x < qtde; x++)
                {
                    exclue = true;


                    for (int y = 0; y < x; y++)
                    {
                        if (quiz[x].tema == quiz[y].tema)
                        {
                            exclue = false;                            
                        }
                        
                    }

                    if (exclue)
                    {
                        Console.Write($"{quiz[x].tema}({qtdeTemas})  ");
                        temas[qtdeTemas] = quiz[x].tema.Trim().ToUpper();
                        qtdeTemas++;                                                                                          
                    }

                   
                }


                Console.SetCursorPosition(0, 28);
                Console.Write("Digite: ");
                escolha = avaliaInt();               
                Console.Clear();

                //esse "do while" ira rodar enquanto a escolha for menor que zero ou maior que a qtde de temas disponiveis.
            } while (escolha > qtdeTemas || escolha < 0);

            //iguala a static string temaEscolhido ao vetor correspondente à escolha do usuario
            temaEscolhido = temas[escolha];
        }

        static void quantasPerguntas()
        {
            do
            {
                Console.Write("Quantas perguntas deseja enfrentar?");
                Console.SetCursorPosition(0, 28);
                Console.Write("Digite: ");
                qtdePerguntas = avaliaInt();
                if (qtdePerguntas > qtdePerguntasTema(temaEscolhido) && temaEscolhido != "-1")
                {
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine($"O tema escolhido tem apenas {qtdePerguntasTema(temaEscolhido)} perguntas.");
                    Console.Beep();
                    Thread.Sleep(2000);

                    ///Apaga o que foi escrito acima.
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("                                                                        ");

                    ///iguala a qtde de perguntas que o programa ira escolher com a quantidade maxima de perguntas do tema.
                    qtdePerguntas = qtdePerguntasTema(temaEscolhido);
                }
            } while (qtdePerguntas > 100 || qtdePerguntas < 0);

        }


        static void Main(string[] args)        
        {
            LePerguntas();
            
            string opcao;

            Console.WriteLine("Esse trabalho foi criado por: ");
            Console.SetCursorPosition(0, 10);
            Console.WriteLine("André Vitor Pereira Cini - 081200039\n\nGustavo Peterlini de Oliveira - 081200046\n\nMurilo Tinari Abdalla - 081200012");

            Console.WriteLine("\n\n\n\n\nDigite ENTER para entrar no jogo.");
            Console.ReadLine();
            
            player.URL = "highwaytohell.mp3";
            player.controls.play();
            player.settings.setMode("loop", true);

            do
            {
                tempoTotal = 0;
                Console.Clear();
                LeRecord();

                //Escreve TM QUIZ no menu
                Console.WriteLine("\n\n\n\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("________________________________________________________________________________________________________________________");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\t\t\t\t|||||||||  ||||    |||     |||||||  |||   || ||| ||||||||");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\t\t\t\t   |||     ||| |||| ||    |||    || |||   || |||     ||| ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\t\t\t\t   |||     |||  ||  ||    |||    || |||   || |||   |||   ");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("\t\t\t\t   |||     |||      ||    ||| || || |||   || |||  |||    ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\t\t\t\t   |||     |||      ||     |||||||   ||||||  ||| ||||||||");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("\t\t\t\t                               ||                        ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("________________________________________________________________________________________________________________________");


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\t\t\t\t Jogar = 1 \t||\t Ajuda = 2 \t||\t Sair = 3  \n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("________________________________________________________________________________________________________________________");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                
                Console.SetCursorPosition(0, 28);
                Console.Write("Digite: ");
                opcao = Console.ReadLine().Trim();

                switch (opcao)
                {

                    case "1":
                        modoJogar();
                        break;

                    case "2":
                        ajuda();
                        break;

                    case "3":
                        break;


                    default:
                        Console.Clear();
                        break;
                }

            } while (opcao != "3");


        }

        static int avaliaInt()
        {
            int n;
            do
            {
                try
                {
                    n = int.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.SetCursorPosition(0, 15);
                    Console.WriteLine("ESCREVA APENAS NÚMEROS!");
                    Console.WriteLine("\nDigite novamente. ");
                    Console.SetCursorPosition(9, 28);
                }
            } while (true);
            return n;
        }

        static void todosTemas()
        {
            
            quantasPerguntas();
            bool verifica = false;
            Random rnd = new Random();
            int[] exclusor = new int[qtdePerguntas];


            for (int x = 0; x < qtdePerguntas; x++)
            {
                verifica = true;

                ///sorteia um numero de pergunta
                exclusor[x] = rnd.Next(0, qtde);

                ///Para nao repetir a mesma pergunta.
                for (int y = 0; y < x; y++)
                {
                    if (exclusor[x] == exclusor[y])
                    {
                        x--;
                        verifica = false;
                    }
                }
                if (verifica)
                {
                    Console.Clear();
                    Console.WriteLine($"Pergunta{x + 1}: {quiz[exclusor[x]].pergunta}");
                    Console.WriteLine($"\nA- {quiz[exclusor[x]].A}     B- {quiz[exclusor[x]].B}     C- {quiz[exclusor[x]].C}     D-{quiz[exclusor[x]].D}");
                    timer(exclusor[x]);
                    Console.Clear();
                }
            }
        }

        //metodo para quando o usuario escolhe a opcao jogar
        static void modoJogar()
        {

            certas = 0;
            erradas = 0;
            string opcao2;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Modo: ");
                Console.WriteLine("\n\n1 -) Casual");
                Console.WriteLine("2 -) Competitivo");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 28);
                Console.Write("Digite: ");
                opcao2 = Console.ReadLine();
                switch (opcao2)
                {
                    case "1":
                        casual();
                        break;

                    case "2":
                        competitivo();
                        break;

                    default:
                        break;

                }
            } while (opcao2 != "1" && opcao2 != "2");


            Console.Clear();
            Console.SetCursorPosition(15, 0);
            checarPontuacao(opcao2);

            player.controls.stop();
            player.URL = "highwaytohell.mp3";
            player.controls.play();
            player.settings.setMode("loop", true);


        }

        static void ajuda()
        {
            Console.Clear();
            Console.WriteLine("Olá, seja bem-vindo ao jogo Tm QUIZ.");

            Console.WriteLine("\nPelo jeito, você deve estar com alguma dúvida em relação ao jogo, mas estamos aqui para te ajudar a sair dessa…");

            Console.WriteLine("\nBasicamente, nosso jogo funciona da seguinte maneira: após entrar na tela inicial(aquela mesma que você clicou na opção ajuda para chegar aqui), " +
                "\ndigite 1 para selecionar a opção 'jogar'.Logo após isso, você deverá escolher um tema e a quantidade de perguntas na qual você deseja enfrentar" +
                "\n(lembrando que os temas e a quantidade de perguntas estão diretamente ligados ao arquivo de texto que estará na pasta do arquivo). " +
                "\nApós isso, você poderá escolher uma dificuldade, que determinará o tempo para responder cada pergunta do quiz(nível fácil - 20 segundos; nível médio" +
                "\n - 15 segundos e nível difícil -10 segundos).");

            Console.WriteLine("\nA partir daí, tudo estará em suas mãos.Você deverá responder a maior quantidade de perguntas certas possíveis e no menor tempo. Caso você tenha feito a maior pontuação e o maior tempo, " +
                "você será o nosso novo recordista!");

            Console.WriteLine("\nAgora, digite enter para sair e dê o seu melhor para bater o recorde e ficar marcado em nossa história!");

            Console.ReadLine();
        }

        //Metodo para rodar o modo casual
        static void casual()
        {
            string opcao1;
            do
            {                
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Modo de jogo: \n\n");
                Console.WriteLine("1 -) Escolher tema\n\n\n\n");
                Console.WriteLine("2 -) Todos os temas");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 28);
                Console.Write("Digite: ");
                opcao1 = Console.ReadLine();
                switch (opcao1)
                {
                    case "1":

                        escolherDificuldade("1");                      
                        Console.Clear();
                        escolheTema();
                        quantasPerguntas();
                        if (tempoPergunta == 10)
                        {
                            player.URL = "thunderstruck.mp3";
                            player.controls.play();
                            player.settings.setMode("loop", true);
                        }
                        geraPergunta();

                        break;

                    case "2":

                        escolherDificuldade("2");                        
                        temaEscolhido = "-1";
                        Console.Clear();
                        if (tempoPergunta == 10)
                        {
                            player.URL = "thunderstruck.mp3";
                            player.controls.play();
                            player.settings.setMode("loop", true);
                        }
                        todosTemas();                        
                        break;
                    default:
                        break;
                }

            } while (opcao1 != "1" && opcao1 != "2");
            pontuacao = certas * ((tempoPergunta * qtdePerguntas) - tempoTotal) * 1000;
            Console.WriteLine($"ACABOU!\n\n\nPONTUAÇÃO: {pontuacao}\nRESPOSTAS CERTAS: {certas}\nRESPOSTAS ERRADAS: {erradas}\nTempo: {tempoTotal:f2} segundos");
            Console.ReadLine();

        }

        //Metodo para rodar o modo competitivo
        static void competitivo()
        {
            
            player.controls.stop();
            
            tempoPergunta = 7;
            qtdePerguntas = 20;
            Console.Clear();
            Console.WriteLine("Nesse modo você terá que responder 20 perguntas em menos de 7 segundos cada.\n\nDe ENTER para começar.");
            Console.ReadLine();

            player.URL = "throughthefireandflames.mp3";
            player.controls.play();
            player.settings.setMode("loop", true);

            for (int x = 5; x > 0; x--)
            {
                Console.Clear();
                Console.SetCursorPosition(15, 14);
                Console.WriteLine($"O jogo começa em: {x} segundos");
                Thread.Sleep(1000);
            }
            

            bool verifica = false;
            Random rnd = new Random();
            int[] exclusor = new int[qtdePerguntas];


            for (int x = 0; x < qtdePerguntas; x++)
            {
                verifica = true;

                ///sorteia um numero de pergunta
                exclusor[x] = rnd.Next(0, qtde);

                ///Para nao repetir a mesma pergunta.
                for (int y = 0; y < x; y++)
                {
                    if (exclusor[x] == exclusor[y])
                    {
                        x--;
                        verifica = false;
                    }
                }
                if (verifica)
                {
                    Console.Clear();
                    Console.WriteLine($"Pergunta{x + 1}: {quiz[exclusor[x]].pergunta}");
                    Console.WriteLine($"\nA- {quiz[exclusor[x]].A}     B- {quiz[exclusor[x]].B}     C- {quiz[exclusor[x]].C}     D-{quiz[exclusor[x]].D}");
                    timer(exclusor[x]);
                    Console.Clear();
                }
            }

        }

        
        static void escolherDificuldade(string opcao1)
        {
            string opcao;
            do
            {
                if (opcao1 == "1")
                {

                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Escolha a dificuldade:\n\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("1 -) Escolher tema");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      1 -) Fácil\n      2 -) Médio\n      3 -) Difícil\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("2 -) Todos os temas");

                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Escolha a dificuldade:\n\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("1 -) Escolher tema\n");
                    Console.WriteLine("2 -) Todos os temas");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      1 -) Fácil\n      2 -) Médio\n      3 -) Difícil");

                }
                Console.SetCursorPosition(0, 28);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Digite: ");
                opcao = Console.ReadLine();

            } while (opcao != "1" && opcao != "2" && opcao != "3");

            if (opcao == "1")
                tempoPergunta = 20;
            else if (opcao == "2")
                tempoPergunta = 15;
            else
            {
                player.controls.stop();                
                tempoPergunta = 10;
            }
        }

        static void checarPontuacao(string opcao)
        {
            string nome;
            

            if (opcao == "2")
            {
                //formula criada para pontuacao. depende da quant de certas e do tempo,
                pontuacao = certas * (140 - tempoTotal) * 1000;                                              

                do
                {
                    Console.WriteLine("ESCREVA SEU NOME PARA ENTRAR NA LISTA DOS RECORDES: ");
                    nome = Console.ReadLine().Trim().ToUpper();

                } while (nome.Length > 10);

                File.WriteAllText("record.txt", record + "\n" + nome + " : " + pontuacao + " |");
                               
                if (pontuacao > primeiro)
                    newRecord();


                LeRecord();

                tabelaRecord(1);
                tabelaRecord(2);
                tabelaRecord(3);
                

                Console.WriteLine("\n\n\nENTER para ver estatísticas");
                Console.ReadLine();
                Console.WriteLine($"Estatísticas\n\n\nPONTUAÇÃO: {pontuacao}\nRESPOSTAS CERTAS: {certas}\nRESPOSTAS ERRADAS: {erradas}\nTEMPO GASTO: {tempoTotal:f2} segundos");
                Console.ReadLine();
                player.controls.stop();

            }
        }
        static void newRecord()
        {            
            player.controls.stop();
            player.URL = "wearethechampions.mp3";
            player.settings.setMode("loop", true);
            Console.Clear();
            Console.WriteLine("\n\n\n\n\n\n\n\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    \t\t        |||||       |||   ||||||||||||   |||        |||        |||");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("    \t\t        ||| ||      |||   |||             |||      |||||      |||");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("    \t\t        |||  |||    |||   ||||||||||||     |||    ||   ||    |||");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("    \t\t        |||    |||  |||   |||               |||  ||     ||  |||");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("    \t\t        |||     ||| |||   |||                || ||       || ||");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("    \t\t        |||       |||||   ||||||||||||        |||         |||");

            Console.WriteLine("\n");


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    \t\t   ||||||||||    ||||||||||  |||||||||   |||||||    ||||||||||   |||||||");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("    \t\t   |||     |||   |||         |||        |||   |||   |||     |||  |||   |||");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("    \t\t   |||||||||||   ||||||||||  |||        |||   |||   |||||||||||  |||   |||");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("    \t\t   ||| |||       |||         |||        |||   |||   ||| |||      |||   |||");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("    \t\t   |||    |||    |||         |||        |||   |||   |||    |||   |||   |||");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("    \t\t   |||      |||  ||||||||||  |||||||||   |||||||    |||      ||| |||||||");
            Thread.Sleep(1000);
            Console.SetCursorPosition(0, 27);
            for (int x = 0; x < 14; x++)
            {

                Console.WriteLine("\n");
                Thread.Sleep(700);
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            
        }

        static void tabelaRecord(int colocacao)
        {

            if (colocacao == 1)
                for (int x = 0; x < qtdeDados; x++)
                {
                    if(pontuacoes[x] == primeiro)
                    {
                        Console.WriteLine($"1º --- {dados[x].Substring(1)}");
                    }
                }
            else if (colocacao == 2)
                for (int x = 0; x < qtdeDados; x++)
                {
                    if (pontuacoes[x] == segundo)
                    {
                        Console.WriteLine($"2º --- {dados[x].Substring(1)}");
                    }
                }
            else
                for (int x = 0; x < qtdeDados; x++)
                {
                    if (pontuacoes[x] == terceiro)
                    {
                        Console.WriteLine($"3º --- {dados[x].Substring(1)}");
                    }
                }
            
        }

    }




    
}
