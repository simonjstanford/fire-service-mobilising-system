#Introduction#

With over ten years of experience at Northamptonshire Fire & Rescue Service (NFRS) Control Room and training in software development, the author of this project was in a unique position to conduct a piece of research that analysed the usefulness of the system currently used by NFRS to log emergency calls and dispatch resources (the ‘mobilising system’).  These webpages contain the results of the research and the resulting WPF application that was developed.

#Research#

Broadly, the results of this research are:

1. Technical issues with the system were identified, and findings show that the method the mobilising system used to calculate the travel times of resources to emergencies was inaccurate when compared to alternative products.  
2. An expert usability testing took place on the mobilising system using Benyon’s twelve design principles (2010), and seventeen usability issues were identified together with a recommendation of how the issue can be resolved.
3. An efficiency analysis was conducted on the mobilising system using Keystroke-Level Modelling.  This mathematically calculated the speed that details of an emergency could be entered.  Using the results of this analysis, a new input window layout was presented, and testing showed that it was significantly quicker than the original design in some situations. 

#Prototype Development#

Following the completion of this research, a prototype .NET Windows Presentation Foundation (WPF) application was developed using the Unified Process model that resolved the technical, usability and efficiency issues identified.

Development of the prototype application consisted of:

1. Analysis of application requirements through workshops with end users and examination of the domain area. 
2. Design modelling that expanded the requirements analysis.
3. An iterative development of the prototype.
4. Participatory usability testing that shaped the final design of the prototype.

A number of technologies were implemented into the prototype:

- The [MVVM] (http://en.wikipedia.org/wiki/Model_View_ViewModel) design pattern.
- [Google Maps API Web Services] (https://developers.google.com/maps/documentation/webservices/) for address searches and travel time calculations.
- The [Bing Maps WPF Control](http://msdn.microsoft.com/en-GB/library/hh750210.aspx) for displaying an embedded map in the application.

#Conclusions#

The results of the research were presented to Northamptonshire Fire & Rescue Service.  Whilst it was not the aim of this project for the prototype to be deployed, it is hoped that the results of the research will assist the service of configuring the user interface of their mobilising system to ensure maximum usability and efficiency.

#More Information#

To view the WPF prototype application, supporting documentation, database definition and additional materials please see the [downloads](https://bitbucket.org/sstanford/optimising-fire-service-emergency-call-handling-and-resource/downloads) page.  The source code is viewable in the [source](https://bitbucket.org/sstanford/optimising-fire-service-emergency-call-handling-and-resource/src) page.