// This script configures the .env.development.local file with additional environment variables to configure HTTPS using the ASP.NET Core
// development certificate in the webpack development proxy.

import fs from 'node:fs';
import path from 'node:path';

const appData = process.env.APPDATA;

const baseFolder = appData !== undefined && appData !== ''
    ? `${appData}/ASP.NET/https`
    : `${process.env.HOME}/.aspnet/https`;

const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
const certificateName = certificateArg?.groups?.value ?? process.env.npm_package_name;

if (!certificateName) {
  console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.')
  process.exit(-1);
}

const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync('.env.development.local')) {
  fs.writeFileSync(
    '.env.development.local',
    `VITE_SSL_CRT_FILE=${certFilePath}\nVITE_SSL_KEY_FILE=${keyFilePath}`,
  );
} else {
  const lines = fs.readFileSync('.env.development.local')
    .toString()
    .split('\n');

  let hasCert = false;
  let hasCertKey = false;
  for (const line of lines) {
    if (/VITE_SSL_CRT_FILE=.*/i.test(line)) {
      hasCert = true;
    }
    if (/VITE_SSL_KEY_FILE=.*/i.test(line)) {
      hasCertKey = true;
    }
  }

  if (!hasCert) {
    fs.appendFileSync(
      '.env.development.local',
      `\nVITE_SSL_CRT_FILE=${certFilePath}`
    );
  }
  
  if (!hasCertKey) {
    fs.appendFileSync(
      '.env.development.local',
      `\nVITE_SSL_KEY_FILE=${keyFilePath}`
    );
  }
}
