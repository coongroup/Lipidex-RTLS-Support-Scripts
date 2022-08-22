# Lipidex-RTLS-Support-Scripts
This library contains the c# support scripts used to enable MSn-tree searching in LipiDex.

Each folder contains a separate C# project that should be run from source in Visual Studio. To use the Coon lab MGF generator, you'll need to install MSFileReader 3.0 and make sure that XRawFile2.dll is registered.

Run order to combine separately searched MSn trees into one consensus lipid spectral match list is (1) Concatenate Spectral Matches (2) Isomer Purity Correction (3) Correct Lipid Precursor Info. You can then plug the resulting spectral match file into LipiDex's Peak Finder.
