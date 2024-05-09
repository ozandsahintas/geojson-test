using Nancy.Diagnostics;
using Nancy.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace GeoJsonParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Root ro = new Root();
            try
            {

                StreamReader sr = new StreamReader(@"c:\test.json");
                string jsonString = sr.ReadToEnd();
                JavaScriptSerializer ser = new JavaScriptSerializer();
                ro = ser.Deserialize<Root>(jsonString);


            }
            catch (Exception ex)
            {

            }


            foreach (var item in ro.features)
            {
                //var a = Task.FromResult(SetCity(item, "sehirler")).Result;
                var a = Task.FromResult(SetDistrict(item)).Result;
            }

            Task.Run(() => SetVehicle("40.2", "28.9", "tasit1", "test")).Wait();
            Task.Run(() => IsIn("test", "sehirler", "Turkiye")).Wait();


            var xx = Task.Run(() => WhereIn("sehirler", "39.558", "26.984")).Result;
            Console.WriteLine(xx);
            var yy = Task.Run(() => WhereIn(xx, "39.558", "26.984")).Result;
            Console.WriteLine(yy);



            Console.ReadLine();
        }


        public static async Task SetCity(Feature item, string cityGroup)
        {
            try
            {
                HttpClient client = new HttpClient();


                HttpRequestMessage requestMessage = new HttpRequestMessage();

                using StringContent jsonContent = new(JsonSerializer.Serialize(item),Encoding.UTF8,"application/json");


                using var response = await client.PostAsync($"http://127.0.0.1:9851/set+{cityGroup}+{item.properties.NAME_1}+object+", jsonContent);

                response.EnsureSuccessStatusCode();

                var resp = await response.Content.ReadFromJsonAsync<Return>();

                Console.WriteLine(resp.ok);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static async Task SetDistrict(Feature item)
        {
            try
            {
                HttpClient client = new HttpClient();


                HttpRequestMessage requestMessage = new HttpRequestMessage();

                using StringContent jsonContent = new(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");


                using var response = await client.PostAsync($"http://127.0.0.1:9851/set+{item.properties.NAME_1}+{item.properties.NAME_2}+object+", jsonContent);

                response.EnsureSuccessStatusCode();

                var resp = await response.Content.ReadFromJsonAsync<Return>();

                Console.WriteLine(resp.ok);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static async Task SetVehicle(string lng, string lat, string name, string vehicleGroup)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage();

                using var response = await client.GetAsync($"http://127.0.0.1:9851/set+{vehicleGroup}+{name}+point+{lng}+{lat}");

                response.EnsureSuccessStatusCode();

                var resp = await response.Content.ReadFromJsonAsync<Return>();
                Console.WriteLine("SetVehicle: " + resp.ok);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static async Task IsIn(string vehicleGroup, string cityGroup, string city)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage();

                using var response = await client.GetAsync($"http://127.0.0.1:9851/within+{vehicleGroup}+get+{cityGroup}+{city}");

                response.EnsureSuccessStatusCode();

                var resp = await response.Content.ReadFromJsonAsync<Return>();
                Console.WriteLine("Is in: " + resp.ok);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static async Task<string> WhereIn(string cityGroup, string lng, string lat)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage();

                using var response = await client.GetAsync($"http://127.0.0.1:9851/intersects+{cityGroup}+point+{lng}+{lat}");

                response.EnsureSuccessStatusCode();

                var resp = await response.Content.ReadFromJsonAsync<Return>();
                Console.WriteLine("Where in: " + resp.ok);

                return resp.objects.FirstOrDefault().id;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return string.Empty;
        }

    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);


    public class ReturnObject
    {
        public string id { get; set; }
        public FeatureObject @object { get; set; }
    }

    public class FeatureObject
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
    }

    public class Return
    {
        public bool ok { get; set; }
        public List<ReturnObject> objects { get; set; }
        public int count { get; set; }
        public int cursor { get; set; }
        public string elapsed { get; set; }
    }

    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<List<double>>>> coordinates { get; set; }
    }

    public class Properties
    {
        public string name { get; set; }
        public string GID_1 { get; set; }
        public string GID_0 { get; set; }
        public string COUNTRY { get; set; }
        public string NAME_1 { get; set; }
        public string NAME_2 { get; set; }
        public string VARNAME_1 { get; set; }
        public string NL_NAME_1 { get; set; }
        public string TYPE_1 { get; set; }
        public string ENGTYPE_1 { get; set; }
        public string CC_1 { get; set; }
        public string HASC_1 { get; set; }
        public string ISO_1 { get; set; }
    }

    public class Root
    {
        public string type { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
        public List<Feature> features { get; set; }
    }


}
