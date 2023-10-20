# VMwareInfoExporter

## Description
VMwareInfoExporter is a C# program designed to fetch and export Virtual Machine (VM) information from a vCenter or an ESXi host to a CSV file. The exported information includes VM name, IP address, Guest OS, and any notes associated with the VM.

## Dependencies
This program relies on the following DLLs from VMware which can be obtained [here](https://www.powershellgallery.com/packages/VMware.Vim/):

- VMware.Vim.dll
- VimService.dll
- VMware.Binding.Wcf.dll
- VMware.Binding.WsTrust.dll

## Installation
1. Download and install the required DLLs mentioned in the Dependencies section.
2. Clone this repository to your local machine.
3. Open the solution using Visual Studio or any other C# IDE of your choice.
4. In the IDE, add the downloaded DLLs as references to your project (Right-click on the project -> Add -> Reference -> Browse to the location of the DLLs -> Add).
5. Build the solution.

## Usage
```bash
VMwareInfoExporter.exe -server <server_address> -outputpath <path_to_csv>
```

- `-server` : The address of the vCenter or ESXi host.
- `-outputpath` : The path to the CSV file where data should be saved.

When you run the program, you will be prompted to enter the username and password for the specified server.

## Output
The program will generate a CSV file with the following columns:

- VM Name
- IP Address
- Guest OS
- Notes

## License
This project is licensed under the GNU Public License 3.0.

## Support
For any issues or suggestions, please feel free to open an issue on GitHub.
