NArrange - "An open source tool for arranging .Net source code"  

It is recommended that you add the NArrange bin folder to your %PATH% 
environment variable.
 
To arrange a file just run...

>narrange-console <sourcefile> [optional output file]

NOTE: If an output file is not specified, the original source file will be 
overwritten. 

Alternatively, you can run NArrange against a C# project file or solution.  
NOTE: When arranging a project or solution, the original source files will 
be overwritten.
 
If you don't like the default settings in DefaultConfig.xml you can copy 
it to a new config and specify the modified configuration file in the 
command line (see narrange-console help).