#! /bin/sh
docs=$(ls ./coveragereport )

for doc in $docs
do
    echo "Sending $doc..."
    curl -H "Token: ${TOKEN}" -H "Content-Type: text/html" -X POST --data "@./coveragereport/$doc" ${REPORT_URI}/${BRANCH_NAME}/$doc
    rm "./coveragereport/$doc"
done