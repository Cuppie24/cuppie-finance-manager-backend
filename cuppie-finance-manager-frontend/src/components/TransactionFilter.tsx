import React, { useState } from "react"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { DateTimePicker } from "@/components/ui/date-time-picker"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet"
import { ChevronDown, X, Filter } from "lucide-react"
import { Checkbox } from "@/components/ui/checkbox"
import { Separator } from "@/components/ui/separator"
import { Badge } from "@/components/ui/badge"

interface TransactionFilters {
  type?: "all" | "income" | "expense"
  categories?: number[]
  fromDate?: Date
  toDate?: Date
}

interface TransactionFiltersProps {
  filters: TransactionFilters
  allCategories: { id: number; name: string }[]
  onChange: (newFilters: TransactionFilters) => void
}

const TransactionFilter: React.FC<TransactionFiltersProps> = ({ filters, allCategories, onChange }) => {
  const [showCategories, setShowCategories] = useState(false)
  const [isOpen, setIsOpen] = useState(false)

  const handleTypeChange = (value: string) => {
    const val = value as "all" | "income" | "expense"
    onChange({ ...filters, type: val === "all" ? undefined : val })
  }

  const handleCategoryToggle = (categoryId: number) => {
    const current = filters.categories ?? []
    const newCategories = current.includes(categoryId)
      ? current.filter((id) => id !== categoryId)
      : [...current, categoryId]
    onChange({ ...filters, categories: newCategories })
  }

  const handleDateChange = (field: "fromDate" | "toDate", date: Date | null) => {
    onChange({ ...filters, [field]: date || undefined })
  }

  const handleReset = () => {
    onChange({})
  }

  // Подсчет активных фильтров
  const activeFiltersCount = [
    filters.type && filters.type !== "all",
    filters.categories && filters.categories.length > 0,
    filters.fromDate,
    filters.toDate
  ].filter(Boolean).length

  return (
    <Sheet open={isOpen} onOpenChange={setIsOpen}>
      <SheetTrigger asChild>
        <Button variant="outline" className="relative">
          <Filter className="w-4 h-4 mr-2" />
          Фильтры
          {activeFiltersCount > 0 && (
            <Badge className="ml-2 h-5 px-1.5 text-xs" variant="default">
              {activeFiltersCount}
            </Badge>
          )}
        </Button>
      </SheetTrigger>
      <SheetContent>
        <SheetHeader>
          <SheetTitle>Фильтры транзакций</SheetTitle>
          <SheetDescription>
            Настройте фильтры для отображения нужных транзакций
          </SheetDescription>
        </SheetHeader>
        <div className="flex flex-col gap-6 py-6">
          {/* Тип операции */}
          <div className="flex flex-col">
            <Label className="mb-2 font-semibold">Тип операции</Label>
            <Select
              value={filters.type ?? "all"}
              onValueChange={handleTypeChange}
            >
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Все</SelectItem>
                <SelectItem value="income">💰 Доход</SelectItem>
                <SelectItem value="expense">💸 Расход</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Категории */}
          <div className="flex flex-col">
            <Label className="mb-2 font-semibold">Категории</Label>
            <Popover open={showCategories} onOpenChange={setShowCategories}>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  className="w-full justify-between"
                >
                  {filters.categories && filters.categories.length > 0
                    ? `${filters.categories.length} выбрано`
                    : "Выберите категории"}
                  <ChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-full p-0">
                <div className="p-2 max-h-64 overflow-y-auto">
                  {allCategories.map((category) => (
                    <label
                      key={category.id}
                      className="flex items-center gap-2 py-2 px-2 rounded hover:bg-accent cursor-pointer"
                    >
                      <Checkbox
                        checked={filters.categories?.includes(category.id) ?? false}
                        onCheckedChange={() => handleCategoryToggle(category.id)}
                      />
                      <span className="text-sm">{category.name}</span>
                    </label>
                  ))}
                  {filters.categories && filters.categories.length > 0 && (
                    <>
                      <Separator className="my-2" />
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => {
                          onChange({ ...filters, categories: [] })
                        }}
                        className="w-full justify-center text-xs"
                      >
                        <X className="w-3 h-3 mr-1" />
                        Очистить
                      </Button>
                    </>
                  )}
                </div>
              </PopoverContent>
            </Popover>
          </div>

          {/* Даты */}
          <div className="flex flex-col gap-3">
            <Label className="font-semibold">Период</Label>
            <div className="flex flex-col gap-4">
              <div className="flex flex-col gap-2">
                <Label className="text-sm text-muted-foreground">От</Label>
                <DateTimePicker
                  selected={filters.fromDate || null}
                  onChange={(date) => handleDateChange("fromDate", date)}
                  showTimeSelect={false}
                  dateFormat="dd.MM.yyyy"
                  placeholderText="Начальная дата"
                />
              </div>
              <div className="flex flex-col gap-2">
                <Label className="text-sm text-muted-foreground">До</Label>
                <DateTimePicker
                  selected={filters.toDate || null}
                  onChange={(date) => handleDateChange("toDate", date)}
                  showTimeSelect={false}
                  dateFormat="dd.MM.yyyy"
                  placeholderText="Конечная дата"
                />
              </div>
            </div>
          </div>

          <Separator />

          {/* Кнопки действий */}
          <div className="flex gap-2">
            <Button
              type="button"
              onClick={handleReset}
              variant="outline"
              className="flex-1"
            >
              <X className="w-4 h-4 mr-2" />
              Сбросить
            </Button>
            <Button
              type="button"
              onClick={() => setIsOpen(false)}
              className="flex-1"
            >
              Применить
            </Button>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  )
}

export default TransactionFilter
