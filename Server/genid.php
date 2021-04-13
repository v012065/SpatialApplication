<?php
/* Randomised ID value
$id = 0;

/* File path */
$filename = "";

/* No duplicate IDs */
do
{
  $id = rand(10000000,99999999);

  $filename = "uploads/recording-{$id}.xml";
}
while(file_exists($filename));

echo($id);
?>
