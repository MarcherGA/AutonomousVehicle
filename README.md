<h2>Setup</h2>
<h4 dir="auto">Requirements:</h4>
<ul dir="auto">
<li>Unity 2022.3.13f1 (other versions are not tested)</li>
</ul>

<h4 dir="auto">Steps:</h4>
<ul dir="auto">
<li>git clone <a href="https://github.com/mofr/Diablerie.git">https://github.com/MarcherGA/AutonomousVehicle.git</a></li>
<li>Run Unity Editor and open AutonomousVehicle folder as a project</li>
<li>In Assets folder open <code>Scenes/Simulation.scene</code> file</li>
<li>Modify <code>config.json</code> for your own configuration</li>
<li>Press <code>Play</code></li>
</ul>

<h4 dir="auto">Build (Simulation):</h4>
<ul dir="auto">
<li>Create Build folder In inside the project folder, If not already created</li>
<li>Create Simulation folder inside Build folder</li>
<li>In Unity Editor, Select <code>File -> Build Settings</code></li>
<li>In Platforms window, Select <code>Windows, Mac, Linux</code> (tested on Windows only) from Platforms</li>
<li>Select your platform on <code>Target Platform</code> Dropdoown list</li>
<li>Press <code>Build And Run</code> and select Simulation folder</li>
<li>The build will be inside Build folder, and will run automatically</li>
</ul>

<h4 dir="auto">Build (Monitor):</h4>
<ul dir="auto">
<li>Create Build folder In inside the project folder, If not already created</li>
<li>Create Monitor folder inside Build folder</li>
<li>In Unity Editor, Select <code>File -> Build Settings</code></li>
<li>In Platforms window, Select <code>Windows, Mac, Linux</code> (tested on Windows only) from Platforms</li>
<li>Select your platform on <code>Target Platform</code> Dropdoown list</li>
<li>Press <code>Build And Run</code> and select Simulation folder</li>
<li>The build will be inside Build folder, and will run automatically</li>
</ul>

<h2>Code Design</h2>
<h4>Simulation</h4>
<ul dir="auto">
<li><code>SimulationManager</code> - Manages the simulation, manipulating the simulation components according to user input</li>
<li><code>SimulationManagerUI</code> - Manages the user interface for the simulation</li>
<li><code>AutonomousVehicle</code> - Conrols the movement of the test car, moving it along a set of waypoints</li>
<li><code>ObjectDetector</code> - Attached to a camera, detecting entities visible and within the camera's frustum. Requires detectable entities to have a collider, and be on a selected Detection Layer, using their tag for categorizing</li>
<li><code>DetectionObserver</code> - Observes an <code>ObjectDetector</code> and sends detection data via UDP</li>
</ul>
<h4>Monitor</h4>
<ul dir="auto">
<li><code>MessageDisplay</code> - Manages the display of messages in a scroll view.</li>
<li><code>UDPDetectedEntityObserver</code> - Inherits <code>UDPMessageObserver</code>, check if message is Detected Entity message, and display it using <code>MessageDisplay</code> if so</li>
</ul>
<h4>UDP Communication</h4>
<ul dir="auto">
<li><code>UDPSender</code> - Sends UDP messages to a remote endpoint, getting the target IP address and Port from config</li>
<li><code>UDPReciever</code> - Receives UDP messages and invokes an event when data is received, getting the target Port from config</li>
<li><code>UDPMessageObserver</code> - Abstract class that represnt observer which listenes to UDP messages, and use it</li>
  
</ul>
<h4>Config</h4>
<ul dir="auto">
<li><code>ConfigLoader</code> - Loads configuration data from a JSON file</li>
<li><code>ConfigData</code> - Respresents Config structure</li>
</ul>


<h2>Attributions</h2>
<p>"SimplePoly City" package - https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899</p>
