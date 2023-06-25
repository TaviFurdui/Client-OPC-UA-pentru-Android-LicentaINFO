# Client OPC UA pentru Android
 Pașii pentru folosirea aplicației Xamarin, împreună cu modelele de AI, sunt:
 1. Descărcarea codului sursă.
 2. Instalarea librăriilor din mediul Visual Studio: OPC.UA.FX, Newtonsoft json, Microcharts, Microcharts.Forms, Refactored.MvvmHelpers, Xamarin.CommunityToolkit, Xamarin.Forms.
 3. Instalarea Keras și Tensorflow pentru modelele din Python.
 4. Rularea codului Python.
 5. API-ul va porni astfel pe un server local, pe portul 7733. Pentru a-l rula pe internet, recomand instalarea librăriei Ngrok. Aceasta face un tunnel din partea de localhost către internet. Apoi, în fișierul C# ChartViewModel, din folderul ViewModels, se va modifica path-ul cu url-ul corespunzător.
 6. După toate acestea, clientul se poate rula, iar toate caracteristicile sale ar trebui să funcționeze normal.


De precizat este faptul că toate pachetele și librăriile menționate mai sus ar trebui să se descarce automat o dată cu codul aplicației, însă dacă se decide descărcarea codului C# și a celui Python separat, se vor urma pașii de mai sus.
