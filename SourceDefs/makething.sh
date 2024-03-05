#!/bin/bash

TARGETDIR="../1.4"


# first parameter is the name of the directory to search in
function parse_bodiesxml () {
   # [[ -z $1 ]] || return # if a parameter wasn't provided

   for file in `find $1 -type f -name "*.bodiesxml"`
   do
      # $file doesn't nee $1 prepended to it
      outfile="${file/%.bodiesxml/.xml}"
      mkdir -p $(dirname $TARGETDIR/$outfile)
      #if [$(date -r ./$file) -ne $(date -r $TARGETDIR/$outfile)]
      if [ $file -nt $TARGETDIR/$outfile ]
      then
         echo "parsing $file"
         perl bodiesxml.pl $file > $TARGETDIR/$outfile
      fi
   done
}
function parse_myxml {
   # [[ -z $1 ]] || return # if a parameter wasn't provided

   for file in `find $1 -type f -name "*.myxml"`
   do
      outfile="${file/%.myxml/.xml}"
      mkdir -p $(dirname $TARGETDIR/$outfile)
      if [ $file -nt $TARGETDIR/$outfile ]
      then
         echo "parsing $file"
         perl myxml.pl $file > $TARGETDIR/$outfile
      fi 
   done
} 
function copy_xml {
   # [[ -z $1 ]] || return # if a parameter wasn't provided

   for file in `find $1 -type f -name "*.xml"`
   do
      outfile=$file
      mkdir -p $(dirname $TARGETDIR/$outfile)
      if [ $file -nt $TARGETDIR/$outfile ]
      then
         echo "copying $file"
         cat $file > $TARGETDIR/$outfile
      fi
   done
}

function copy_txt {
   # [[ -z $1 ]] || return # if a parameter wasn't provided

   for file in `find $1 -type f -name "*.txt"`
   do
      outfile=$file
      mkdir -p $(dirname $TARGETDIR/$outfile)
      if [ $file -nt $TARGETDIR/$outfile ]
      then
         echo "copying $file"
         cat $file > $TARGETDIR/$outfile
      fi
   done
}




parse_bodiesxml "Defs"
parse_myxml "Defs"
copy_xml "Defs"

copy_txt "Socities"
