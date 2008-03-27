NArrange - "An open source tool for arranging .Net source code"  

***WARNING***
THIS PROGRAM MODIFIES SOURCE CODE.  BECAUSE IT IS POSSIBLE THAT BUGS EXIST IN THE PROGRAM, IT IS HIGHLY RECOMMENDED THAT YOU CREATE A BACKUP OF YOUR ORIGINAL SOURCE CODE FILES PRIOR TO RUNNING NARRANGE AGAINST THEM.

To ease command line usage, it is also recommended that you add the NArrange bin folder to your %PATH% environment variable.


ARRANGING FILES
--------------- 

To arrange a file just run...

>narrange-console <sourcefile> [optional output file]

NOTE: If an output file is not specified, the original source file will be overwritten. 

Alternatively, you can run NArrange against a C# project file or solution.  
NOTE: When arranging a project or solution, the original source files will be overwritten.


BACKUP
------

To automatically create a backup of source prior to arranging elements, pass the /b backup parameter.
To restore a prior backup, pass the /r restore parameter.


CONFIGURATION
-------------

If you don't like the default settings in DefaultConfig.xml you can copy it to a new config and specify the modified configuration file in the 
command line (see narrange-console help).  
NOTE:  Modifying DefaultConfig.xml will not override settings.  DefaultConfig.xml is provided as an example.  You must specify the configuration file through the /c:configuration command argument.


CLOSING COMMENTS
----------------

To enable closing comments, add the following element under the CodeConfiguration root in the XML config file:

<ClosingComments Enabled="true" Format="End $(ElementType) $(Name)"/>

For an example, see DefaultConfig.xml in the bin directory.  Other valid format variables include $(Access), $(Type) and $(Modifer).


 
