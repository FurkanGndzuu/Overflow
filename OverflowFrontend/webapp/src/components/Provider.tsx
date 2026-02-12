
'use client';
import * as React from "react";

// 1. import `HeroUIProvider` component
import {HeroUIProvider, ToastProvider} from "@heroui/react";
import {ThemeProvider as NextThemesProvider} from "next-themes";
import { ReactNode } from "react";
import { useRouter } from "next/navigation";

export default function App({children}: {children: ReactNode}) {

    const router = useRouter();
  // 2. Wrap HeroUIProvider at the root of your app
  return (
    <HeroUIProvider navigate={router.push} className='h-full flex flex-col'>
      <ToastProvider />
      <NextThemesProvider attribute="class" defaultTheme="light">
        {children}
      </NextThemesProvider>
    </HeroUIProvider>
  );
}