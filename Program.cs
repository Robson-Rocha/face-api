namespace Exemplos.FaceApi
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().Wait();
        }

        public static async Task AsyncMain()
        {
            //Pré-popula o banco de dados com imagens de teste
            await Util.AddPicturesToDb();

            //Conecta ao banco de dados de teste
            using (Db db = Util.GetDb())
            {
                //Itera cada um dos registros
                foreach(var person in db.Persons)
                {
                    System.Console.WriteLine($"Processing {person.Id}...");

                    //Invoca a Face Api para detectar o rosto e características faciais relevantes (gênero e idade) 
                    dynamic detectedData = await FaceApi.Detect(person.Picture);

                    //Continua apenas se algum rosto foi detectado
                    if(detectedData.Count == 0)
                    {
                        Console.WriteLine(" no face detected! Removing picture...");
                        person.Picture = null;
                        continue;
                    }

                    //Obtém o primeiro retângulo de rosto (os demais são ignorados nesta demonstração)
                    int faceRectTop = Convert.ToInt32(detectedData[0].faceRectangle.top.ToString()); 
                    int faceRectLeft = Convert.ToInt32(detectedData[0].faceRectangle.left.ToString()); 
                    int faceRectWidth = Convert.ToInt32(detectedData[0].faceRectangle.width.ToString());
                    int faceRectHeight = Convert.ToInt32(detectedData[0].faceRectangle.height.ToString());

                    //Obtém as características faciais identificadas (pode não ser preciso)
                    person.Gender = detectedData[0].faceAttributes.gender.ToString().Substring(0,1).ToLower();
                    person.Age = (int)Math.Round(Convert.ToDecimal(detectedData[0].faceAttributes.age.ToString().ToLower()));

                    //Corta, redimensiona e converte a imagem com base no rosto detectado
                    person.Picture = ImageManipulation.CropRectangleToSmallerSide(person.Picture.ToBitmap(), faceRectTop, faceRectLeft, faceRectWidth, faceRectHeight)
                                                      .Resize(256, 256)
                                                      .ToPngByteArray();

                    System.Console.WriteLine($" done: {person.Age}, {person.Gender}\r\n");
                }

                //Salva as alterações no banco de dados
                await db.SaveChangesAsync();
            }
        }
    }
}
