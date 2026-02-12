import { AcademicCapIcon } from "@heroicons/react/24/solid";
import Link from "next/link";
import ThemeSwitcher from "./ThemeToggle";
import SearchInput from "./SearchInput";


export default async function TopNav() {
    
    
    return (
        <header className="p-2 w-full fixed z-50 top-0 border-b bg-white dark:bg-black">
            <div className="flex px-10 mx-auto">
                <div className="flex items-center gap-6">
                    <Link href='/'
                          className="flex items-center gap-3 max-h-16">
                        <AcademicCapIcon className="h-10 w-10 text-secondary" />
                        <h3 className="text-xl font-semibold uppercase">Overflow</h3>
                    </Link>
                    <nav className="flex gap-3 my-2 text-md text-neutral-500">
                        <Link href='/'>About</Link>
                        <Link href='/'>Products</Link>
                        <Link href='/'>Contact</Link>
                    </nav>
                </div>

                <SearchInput />

                <div className="flex grow justify-end items-center gap-4">
            <ThemeSwitcher />
            </div>
                
            </div>
        </header>
    );
}