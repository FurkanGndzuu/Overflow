import React from 'react'

import TagCard from "@/app/tags/TagCard";

import { TagAction } from '@/lib/actions/tag-action';
import TagPageHeader from './TagsHeader';



export default async function page() {
  
    
    const {data: tags, error} = await TagAction();

    if (error) throw error;

    return (
        <div className='w-full px-6'>
            <TagPageHeader />
            <div className='grid grid-cols-3 gap-4'>
                {tags?.map(tag => (
                    <TagCard key={tag.id} tag={tag}/>
                ))}
            </div>
        </div>
    )
}