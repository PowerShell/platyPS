# Schema for About_ files

The following sections outline schema changes for the About_ file.

|      Heading      | Level | Required? | Count |                                                        Description                                                        |
| ----------------- | ----- | --------- | ----- | ------------------------------------------------------------------------------------------------------------------------- |
| Title             | H1    | Y         | 1     | - Title should be Sentence Case - About Topic<br>- Title meta data 'about_<Topic>' should match the file basename         |
| Short Description | H2    | Y         | 1     | - Should be Sentence Case                                                                                                 |
| Long Description  | H2    | Y         | 1     | - Should be Sentence case<br>- Should allow multiple Long description subtopics<br>- Should support subtopics at H3 or H2 |
| See Also          | H2    | Y         | 1     | - This is required but may be empty                                                                                       |

General notes

- Should be rendered as plain text compatible with `Get-Help`
- `Get-Help` bug:Synopsis
- [Get-Help bug][05]
- Add switch to provide Cabs or Zips. Default: cabs
- Add switch to include markdown Default: off
- About_ schema does not say anything about line wrapping etc.
  - If left as text files, then wrap at 80 columns.
  - But if converted to schema-based line limit isn't a problem (for the future). Still a problem
    for previous versions.
