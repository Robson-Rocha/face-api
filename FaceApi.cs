namespace Exemplos.FaceApi
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public static class FaceApi
    {
        //Define a URL da operação Detect da Face API, parametrizando que não deve ser retornado o FaceId,
        //não devem ser retornados os marcos da face (posição dos olhos, lábios e etc) e que devem ser retornados
        //os atributos da face Gênero e Idade (apenas estimativas)
        private static readonly string _detectUri = 
            $"{Util.Configuration["faceApi:detect:baseUrl"]}?returnFaceId=false&returnFaceLandmarks=false&returnFaceAttributes=gender,age";

        //Obtém a chave de inscrição da Face API (obtenha a sua em https://azure.microsoft.com/try/cognitive-services/)
        private static readonly string _faceApiSubscriptionKey = Util.Configuration["faceApi:subscriptionKey"];

        //Invoca a operação Detect da Face API, enviando uma imagem e retornando a face detectada e suas carcterísticas
        public async static Task<dynamic> Detect(byte[] sourcePictureByteArray)
        {
            //Cria um novo corpo de requisição baseado na array de bytes da imagem
            using (ByteArrayContent content = new ByteArrayContent(sourcePictureByteArray))
            {
                //Define o tipo de conteúdo do envio
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                //Define a chave de inscrição da Face API
                content.Headers.Add("Ocp-Apim-Subscription-Key", _faceApiSubscriptionKey);

                //Faz a requisição à FaceAPI e deserializa o JSON dos resultados
                HttpResponseMessage response = await Util.HttpClient.PostAsync(_detectUri, content);
                dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                //Força um untervalo de 3 segundos para evitar que mais do que 20 transações por minuto sejam executadas
                //(chaves trial possuem 30000 transações em 30 dias, sendo no máximo 30 po minuto)
                //Comente esta linha para chaves de produção
                await Task.Delay(3000);

                //Retorna os resultados obtidos
                return result;
            }
        }
    }
}