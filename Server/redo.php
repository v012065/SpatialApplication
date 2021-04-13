<?php
/* PUT data comes in on the stdin stream */
$putdata = fopen("php://input", "r");

/* Randomised ID value */
$id = fgets($putdata);
$id = str_replace("\n","",$id);

$filename = "uploads/recording-{$id}.xml";

/* Only write if no */
if(file_exists($filename))
{
  /* Open a file for writing */
  $save = fopen($filename, "w");

  /* Read the data 1 KB at a time
   and write to the file */
  while ($data = fread($putdata, 1024))
    fwrite($save, $data);

  /* Close the streams */
  fclose($save);
  fclose($putdata);
}

echo($id);
?>
