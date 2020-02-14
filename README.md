# CatenarySpans
Desktop application for the shape of hanging cable among multiple support points

# Screenshots

 - Main Screen

   ![MainScreen](JA-CatenarySpans-scr2.png)
   
 - Span Details

   ![Details](JA-CatenarySpans-scr3.png)
   
 - Printout

   ![Printout](JA-CatenarySpans-scr1.png)   
   
 # Development
 
 `C#` application in `WinForms` built in `VS2017`.  Project
 includes source files and unit tests.
 
 # Mathematics
 
 The general shape of hanging cable is that of a catenary described by the **cosh(x)** function 
 or more specifically the equation below:
 
  ![shape](y.png)
  
  The program can find the parameters of this equation to fit between the support points even 
  when the span is not even. In addition, the tension needed can be found for a specific clearance or sag amount.
