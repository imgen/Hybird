# Hybird
A cross platform hybrid framework for mobile app using *Xamarin Forms* that allows async JS/Native interaction.

It's not a standalone library, but instead a sample of how to do hybrid app in *Xamarin Forms*, thus cross platform.

The framework consists two components, a js file named `hybrid.js` which is responsible for interaction between *Xamarin* and `WebView`, and the *Xamarin* counterpart `HybridWebView.cs`. 

On *Xamarin* side, it defines a bunch of commands which is available for *WebView* to invoke. Each invokation will generate a new call ID so that each call can be identified. And vice versa, *WebView* can define commands to be invoked from *Xamarin* side. 

For more information, please see the source code.
