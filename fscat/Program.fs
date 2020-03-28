(* Concats multiple F# source files from a project into a single file. *)

open System
open System.IO
open System.Xml
open System.Xml.XPath

type SourceFile = { fullPath: string; moduleName: string }

(* If the path is an existing file, ending with .fs, returns Some SourceFile. Otherwise, None *)
let fullPathToSourceFile (fullPath: string) =
    if File.Exists fullPath
    then
        let fileName = Path.GetFileName fullPath
        let extension = (Path.GetExtension fullPath).ToLowerInvariant()
        if extension = ".fs"
        then Some { fullPath = fullPath; moduleName = fileName.Substring(0, fileName.Length - extension.Length) }
        else None
    else None

(* List of the full path of compiled source files from the project *)
let fsSourceFilesFromProj (fsprojPath: string) =

    (* Retrieves the path of the fsproj file. It is the base path for the source files. *)
    let projPath = fsprojPath |> Path.GetFullPath |> Path.GetDirectoryName

    (* Use an XPath epxression to select the potential source file names *)
    let document = new XPathDocument(fsprojPath)
    let navigator = document.CreateNavigator();  

    let iter = navigator.Select("//ItemGroup/Compile/@Include")
    [ while iter.MoveNext() do yield Path.Combine(projPath, iter.Current.Value) ]
    |> List.choose fullPathToSourceFile

(* Copy the contents of the file to the writer, adding the indentation before each line *)
let appendFile (skipLines: int) (indentation: string) (writer: StreamWriter) (fullPath: string) =
    let copyLine (line: string) =
        writer.Write(indentation)
        writer.WriteLine(line)

    File.ReadAllLines(fullPath)
    |> Array.toSeq
    |> Seq.skip skipLines
    |> Seq.iter copyLine

(* Process a module file *)
let indentFile (writer: StreamWriter) (sourceFile: SourceFile) =
    writer.WriteLine(sprintf "module %s =" sourceFile.moduleName)
    appendFile 1 "    " writer sourceFile.fullPath
    writer.WriteLine()

(* Process the last file (main): simple copy *)
let copyFile (writer: StreamWriter) (sourceFile: SourceFile) =
    appendFile 0 "" writer sourceFile.fullPath

(* Concats the files and write them to outputFileName *)
let combineFiles (outputFileName: string) (sourceFiles: SourceFile list) =
    use writer = new StreamWriter(outputFileName)
    let nbFiles = List.length sourceFiles

    if nbFiles = 0
    then failwith "No F# source found"

    (* Add the module statement and indent *)
    List.take (nbFiles - 1) sourceFiles |> List.iter (indentFile writer)

    (* Last file (main file, such as Program.fs): appends without any change *)
    copyFile writer sourceFiles.[nbFiles - 1]

[<EntryPoint>]
let main argv =
    if (Array.length argv) <> 2
    then failwith "Incorrect number of arguments. fscat my.fsproj the_output.fs"
    elif not (File.Exists argv.[0])
    then failwithf "Could not find %s" argv.[0]
    else
        let fsprojPath = argv.[0]
        let outputFileName = argv.[1]

        fsSourceFilesFromProj fsprojPath
        |> combineFiles outputFileName
        0 (* Exit code: OK *)
