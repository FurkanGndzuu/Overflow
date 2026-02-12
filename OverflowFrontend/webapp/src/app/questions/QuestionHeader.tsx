'use client';
import { Button, Tabs , Tab } from '@heroui/react';
import React from 'react'
import Link from 'next/link';

type Props = {
    tag?: string;
    total:number;
}


export default function QuestionHeader({tag, total}: Props) {

    const tabs = [
        {id : 'newest', label: 'Newest'},
        {id : 'active', label: 'Active'},
        {id : 'unanswered', label: 'Unanswered'},
    ]

  return (
    <div className='flex flex-col w-full border-b gap-4 pb-4'>
        <div className='flex justify-between px-6'>
            <div className='text-3xl font-semibold'>
                {tag ? `${tag} Questions` : 'All Questions'}
            </div>
            <Button color='secondary' as={Link} href='/questions/ask'>
                Ask Question
            </Button>
        </div>
        <div className='flex justify-between px-6 items-center'>
       <div>{total === 1 ? `${total} question` : `${total} questions`}</div>
         <div className='flex items-center'>
           <Tabs>
                {tabs.map(tab => (
                    <Tab
                        key={tab.id}
                        title={tab.label} />
                ))}
           </Tabs>
       </div>
        </div>
    </div>
  )
}
       

