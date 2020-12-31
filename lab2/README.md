# OS lab2

## Page allocator

In C++ computer programming, allocators are a component of the C++ Standard Library. The standard library provides several data structures, such as list and set, commonly referred to as containers. A common trait among these containers is their ability to change size during the execution of the program. To achieve this, some form of dynamic memory allocation is usually required. Allocators handle all the requests for allocation and deallocation of memory for a given container. The C++ Standard Library provides general-purpose allocators that are used by default, however, custom allocators may also be supplied by the programmer.

Any class that fulfills the allocator requirements can be used as an allocator. In particular, a class A capable of allocating memory for an object of type T must provide the types A::pointer, A::const_pointer, A::reference, A::const_reference, and A::value_type for generically declaring objects and references (or pointers) to objects of type T. It should also provide type A::size_type, an unsigned type which can represent the largest size for an object in the allocation model defined by A, and similarly, a signed integral A::difference_type that can represent the difference between any two pointers in the allocation model.

Although a conforming standard library implementation is allowed to assume that the allocator's A::pointer and A::const_pointer are simply typedefs for T* and T const*, library implementors are encouraged to support more general allocators.

As stated, the allocator maintains blocks of free pages where each block is a power of two number of pages. The exponent for the power of two sized block is referred to as the order. An array of free_area_t structs are maintained for each order that points to a linked list of blocks of pages that are free as indicated by Figure:

![sheme1]()

Hence, the 0th element of the array will point to a list of free page blocks of size 2^0 or 1 page, the 1st element will be a list of 2^1 (2) pages up to 2^(MAX_ORDER−1) number of pages, where the MAX_ORDER is currently defined as 10. This eliminates the chance that a larger block will be split to satisfy a request where a smaller block would have sufficed. The page blocks are maintained on a linear linked list via page→list.

Each zone has a free_area_t struct array called free_area[MAX_ORDER]

### Allocating Pages

Linux provides a quite sizable API for the allocation of page frames. All of them take a gfp_mask as a parameter which is a set of flags that determine how the allocator will behave. The flags are discussed in Section 6.4.

The allocation API functions all use the core function \_\_alloc_pages() but the APIs exist so that the correct node and zone will be chosen. Different users will require different zones such as ZONE_DMA for certain device drivers or ZONE_NORMAL for disk buffers and callers should not have to be aware of what node is being used. A full list of page allocation APIs are listed in Table:

![sheme2]()

Allocations are always for a specified order, 0 in the case where a single page is required. If a free block cannot be found of the requested order, a higher order block is split into two buddies. One is allocated and the other is placed on the free list for the lower order. Next figure shows where a 2^4 block is split and how the buddies are added to the free lists until a block for the process is available.

![sheme3]()

### Free pages

For the freeing of pages is a lot simpler and exists to help remember the order of the block to free as one disadvantage of a buddy allocator is that the caller has to remember the size of the original allocation. The API for freeing is listed in Table

![sheme4]()

### GFP flags

A persistent concept through the whole VM is the Get Free Page (GFP) flags. These flags determine how the allocator and kswapd will behave for the allocation and freeing of pages. For example, an interrupt handler may not sleep so it will not have the \_\_GFP_WAIT flag set as this flag indicates the caller may sleep.
The first of the three is the set of zone modifiers listed in Table. These flags indicate that the caller must try to allocate from a particular zone. The reader will note there is not a zone modifier for ZONE_NORMAL. This is because the zone modifier flag is used as an offset within an array and 0 implicitly means allocate from ZONE_NORMAL.
![sheme5]()
