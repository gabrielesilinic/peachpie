<?php

namespace ini\parse_ini;

function test() {
    $ini_content = <<<INI

;no sections
# comments

foo=bar
1 = intkey
constant = PHP_CONST_DEFINED_PREVIOUSLY
result = PHP_CONST_1 | PHP_CONST_2 | PHP_CONST_4
testempty=   
testwithcomments1=bar ;test
testwithcomments2=bar #test

[SECTION_TEST_STRING]
foo=also bar
normal=Hello World
quoted="Hello World"

[SECTION_TEST_NUMBER]
testnum=42
testfloat1=+1.4E-3
testfloat2=0.005

INI;

    define('PHP_CONST_DEFINED_PREVIOUSLY', 'FOOBAR'); 
    define('PHP_CONST_1', '1'); 
    define('PHP_CONST_2', '2'); 
    define('PHP_CONST_4', '4'); 

    echo "Test default".PHP_EOL;
    print_r(parse_ini_string($ini_content));

    echo "Test default with section".PHP_EOL;
    print_r(parse_ini_string($ini_content, true));
}

test();
