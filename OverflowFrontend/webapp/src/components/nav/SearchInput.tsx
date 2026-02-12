"use client";

import { SearchQuestions } from "@/lib/actions/question-action";
import { Question } from "@/lib/types";
import { MagnifyingGlassIcon } from "@heroicons/react/24/solid";
import { Input, Listbox, ListboxItem, Spinner } from "@heroui/react";
import {  useEffect, useRef, useState } from "react";


export default function SearchInput() {


    const [query, setQuery] = useState('');
    const [loading, setLoading] = useState(false);
    const [results, setResults] = useState<Question[] | null>(null);
    const [showDropdown, setShowDropdown] = useState(false);
    const Timeout = useRef<NodeJS.Timeout | null>(null);

    useEffect(() => {
        if(Timeout.current) {
            clearTimeout(Timeout.current);
        }
        if(query.length === 0) {
            // eslint-disable-next-line react-hooks/set-state-in-effect
            setResults(null);
            setShowDropdown(false);
            return;
        }

        Timeout.current = setTimeout(async () => {
            setLoading(true);
            const {data : questions} = await SearchQuestions(query);
            setResults(questions);
            setShowDropdown(true);
            setLoading(false);
        }, 500);
    }, [query]);

      const onAction = () => {
        setQuery('');
        setResults(null);
    }

  return( <div className='flex flex-col w-full mr-8'>
            <Input
                startContent={<MagnifyingGlassIcon className='size-6' />}
                className="ml-6"
                type='search'
                placeholder='Search'
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                endContent={loading && <Spinner size='sm' />}
            />
            {showDropdown && results && (
                <div className='absolute top-full z-50 bg-white dark:bg-default-50 shadow-lg border-2 border-default-500 w-[50%]'>
                    <Listbox
                        onAction={onAction}
                        items={results}
                        className='flex flex-col overflow-y-auto'
                    >
                        {(question) => (
                            <ListboxItem
                                href={`/questions/${question.id}`}
                                key={question.id}
                                startContent={
                                    <div className='flex flex-col h-14 min-w-14 justify-center items-center border border-success rounded-md'>
                                        <span>{question.answerCount}</span>
                                        <span className='text-xs'>answers</span>
                                    </div>
                                }
                            >
                                <div>
                                    <div className='font-semibold'>{question.title}</div>
                                    <div className="text-xs font-semibold opacity-60 line-clamp-2">
                                        {question.context}
                                    </div>
                                </div>
                            </ListboxItem>
                        )}
                    </Listbox>
                </div>
            )}
        </div>
        
    );
}