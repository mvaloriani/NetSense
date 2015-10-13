# NetSense


NetSense is a simplified library to start to use RealSense Camera in c# project.
NetSense is based on "Stream/Frame paradigm" and support MVVM and Events.

Nuget package can be downloaded from: https://www.nuget.org/packages/NetSense
or using the command:  PM> Install-Package NetSense

"Samples" floders contatains:
  * Gestures project
  * Background removal project
  * Color camera project
  * others cooming soon

In the "ProjectTemplate" folder you find a VS2015 project template, usefull to start new blank projects.

If you want go more on dettail you can find in the Core project all the code of the my implementation.


#How to use NetSense

<pre><code>
// Initialize sensor source
RealSenseSensor sensor = new RealSenseSensor();
sensor.InitializeColorStrem(RealSenseColorFormat.Color640x480F30);
sensor.InitializeGestureRecognition();


// Initialize sensor manager
RealSenseManager manager = new RealSenseManager(null);
manager.Initialize(sensor);


// Enable streams
manager.EnableColorStream();

manger.EnableGestureStream();
manager.GeneralGestureRecognizedRaiseEvent += (object sender, GestureEventArgs e) =>   {
     Console.WriteLine (e.gestureName + " - " + e.bodySideType)
}

</code></pre>
