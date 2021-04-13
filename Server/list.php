 <?php
$dir = "uploads";

// Sort in ascending order - this is default
$a = scandir($dir);

foreach($a as $result) {
    echo $result, '<br>';
}
?> 