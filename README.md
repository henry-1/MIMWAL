# MIM WAL

The MIMWAL is a Workflow Activity Library (WAL) for building complex workflows for Microsoft Identity Manager (MIM) 2016.

This project is a fork of Microsoft's MIM WAL repo which can be found [here](https://github.com/microsoft/MIMWAL).

The goal of this project is to remove PowerShell 2 dependency, so that MIM WAL can run on newer operating systems as it will be supported by Microsoft.

# Features
 * Except for PowerShell Workflow activity the library has the same feature set as before.
 * PowerShell 2 dependency was completely removed.
 * PowerShell 5 support has been added.
 * PowerShell 7 support has been added.

# Supported Product Versions
* Microsoft Identity Manager (MIM) 2016 - 4.3.1935 and above (which is inherited from original MIM WAL).
  * MIM 2016 SP3
  	* SharePoint 2019 SE
	* Server 2022
	* Server 2025 (as soon as it is officially supported by Microsoft)

# Release Notes
* Release notes of this fork can be found in the [Change Log](./ChangeLog.md).
* Version history of former releases can be found [here](https://github.com/Microsoft/MIMWAL/blob/master/ChangeLog.md)


# Build Note
The project targets .NET Framework 3.5 as required by the MIM/FIM workflow runtime. When building with newer versions of MSBuild, reference-resolution warnings may be reported for some Microsoft MIM assemblies because their dependency metadata references newer framework assemblies. These warnings do not prevent the project from building.

# Upgrade
The upgrade procedure from previous MIMWAL versions remains unchanged.
