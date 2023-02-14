# Lipidex RTLS Support Scripts

#### Purpose of This Software
This library contains C# support scripts used to enable MS<sup>n</sup>-tree searching in [LipiDex](https://github.com/coongroup/LipiDex). 

#### Why is This Software Needed
The original release of LipiDex has no way to filter its spectral libraries by polarity, collision type, collision energy, or MS<sup>n</sup> order, and it places the impetus on the user to generate extremely curated spectral libraries specific to their exact acquisition technique. This led to scenarios where library entries would be incorrectly compared to experimental data, causing impossible spectral matches were being made.[^note1] 

While these matches usually didn't persist though LipiDex's data filtering steps, these obviously incorrect spectral matches were worrisome and were a constant source of confusion. Further, LipiDex has no method for integrating the additional structural information provided by additional MS<sup>n</sup> acquired on the same lipid feature. We needed a solution to both the above problems. 

#### Our Stopgap Solution
Integrating spectral library filtering was not possible short of a full rebuild of LipiDex. As such, our solution was more along the lines of a side-hack and is unfortunately limited to only Thermo .RAW files (sorry). 

To briefly summarize our strategy, instead of using a spectral conversion program like [MSConvert](https://proteowizard.sourceforge.io/tools/msconvert.html) to generate a set of text-based spectral files containing *all* of the tandem mass spectra from an experiment, we instead convert and group the tandem MS by activation type and MS<sup>n</sup> level.[^note2] Next, we search these spectral groups using only libraries curated to each activation type and MS<sup>n</sup> level using [Spectrum Searcher](https://github.com/coongroup/LipiDex/wiki/Searching-MS-MS-Spectra-for-Lipid-IDs) in LipiDex. Finally, the spectral matches from each of these groups are manipulated to ensure precursor information is properly tracked and are stiched back together into a single final spectral match file which can be piped into [Peak Finder](https://github.com/coongroup/LipiDex/wiki/Identifying-Lipid-Chromatographic-Peaks). 

#### Software Run Order


[^note1]:
    *e.g.* [M-H+acetate]<sup>-</sup> spectral matches in positive mode.
[^note2]:
    Currently this is hardcoded to HCD MS<sup>2</sup>s, CID MS<sup>2</sup>s, and CID MS<sup>3</sup>s to support our work on Real-Time Library Searching.

Each folder contains a separate C# project that should be run from source in Visual Studio. To use the Coon lab MGF generator, you'll need to install MSFileReader 3.0 and make sure that XRawFile2.dll is registered.

Run order to combine separately searched MSn trees into one consensus lipid spectral match list is (1) Concatenate Spectral Matches (2) Isomer Purity Correction (3) Correct Lipid Precursor Info. You can then plug the resulting spectral match file into LipiDex's Peak Finder.
