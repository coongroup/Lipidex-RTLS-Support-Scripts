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
1. Coon Lab MSn Spectrum Combiner - Splits single raw file into HCD MS<sup>2</sup>s, CID MS<sup>2</sup>s, and CID MS<sup>3</sup>s
2. LipiDex Peak Finder - Search each group of MGFs seperately using curated libraries.[^note3]
3. Concatenate Spectral Matches - Takes the spectral matches originally belonging to each experiment and reassembles them into one file.
4. Isomer Purity Correction - If your library contains lipids with positional isomers (*e.g.* PC 18:0/16:1 and PC 16:1/18:0), corrects purity calculation.
5. Correct Lipid Precursor Info - Changes MS<sup>3+</sup> precursor *m/z* values to the original intact precursor.[^note4]
6. LipiDex Peak Finder - Continue data processing as normal.

#### Software Run Instructions
Each folder contains a separate C# project that should be run from source (\*.sln) in Visual Studio. The Coon Lab MSn Spectrum Combiner has a GUI, the other programs need to be run from command line. To use the Coon Lab MSn Spectrum Combiner, you'll need to install MSFileReader 3.0 and make sure that XRawFile2.dll is registered in both 32- and 64-bit forms.

[^note1]:
    *e.g.* [M-H+acetate]<sup>-</sup> spectral matches in positive mode.
[^note2]:
    Currently this is hardcoded to HCD MS<sup>2</sup>s, CID MS<sup>2</sup>s, and CID MS<sup>3</sup>s to support our work on Real-Time Library Searching.
 [^note3]:
    Ideally only the last MS<sup>n</sup> of each fragmentation tree should be marked as *optimal polarity*. This prevents weird behavior with how LipiDex treats serially collected MS<sup>n</sup> scans. LipiDex assumes serially collected MS<sup>n</sup> are "co-isolated" species and collapses these identifications to sum composition (which defeats the purpose of serially collected MS<sup>n</sup>).
[^note4]:
    LipiDex matches liquid chromatography peaks via intact precursor *m/z* in Peak Finder. If this step is skipped, any spectral matches using the MS<sup>3+</sup> isolation *m/z* will likely not make it through Peak Finder correctly. If they do match, they still will be filtered out as in-source fragments.
